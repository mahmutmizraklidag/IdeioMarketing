namespace IdeioMarketing.MarketingFeature.Content
{
    /// <summary>
    /// Marketing kayıtlarının yalnızca sunucu API'si üzerinden kalıcılaştırılmasını sağlar.
    /// </summary>
    public static class MarketingDatabasePersistenceHtmlEnhancement
    {
        private const string StoreStartMarker = "const Store={";
        private const string CommitMarker = "function commit(){Store.save(S.leads);render();}";

        private const string DatabaseStore = """
const Store={
  async load(){
    if(!window.storage)return null;
    try{
      const r=await window.storage.get(KEY);
      return r&&r.value?JSON.parse(r.value):null;
    }catch(e){
      console.error("Marketing verisi veritabanından yüklenemedi.",e);
      return null;
    }
  },
  async save(d){
    if(!window.storage)throw new Error("Marketing veritabanı bağlantısı bulunamadı.");
    await window.storage.set(KEY,JSON.stringify(d));
  }
};

const S={leads:SEED.slice(),view:"dashboard",q:"",fStage:"all",fOwner:"all",fStatus:"all",fMonth:"all",perfMonth:"all",salesMode:"toplam"};
let dragId=null;
async function commit(){await Store.save(S.leads);render();}
""";

        public static string Apply(string html)
        {
            ArgumentNullException.ThrowIfNull(html);

            var start = html.IndexOf(StoreStartMarker, StringComparison.Ordinal);
            var commitStart = html.IndexOf(CommitMarker, start, StringComparison.Ordinal);
            if (start < 0 || commitStart < 0)
            {
                throw new InvalidOperationException("Marketing HTML şablonunda storage kayıt bloğu bulunamadı.");
            }

            var end = commitStart + CommitMarker.Length;
            return string.Concat(html.AsSpan(0, start), DatabaseStore, html.AsSpan(end));
        }
    }
}
