namespace IdeioMarketing.MarketingFeature.Content
{
    /// <summary>
    /// Sıkıştırılmış pazarlama ekranına pipeline görünürlüğü kontrollerini ekler.
    /// </summary>
    public static class MarketingPipelineHtmlEnhancement
    {
        private const string Styles = """
<style id="pipeline-visibility-styles">
  .pcard-actions{display:flex;align-items:center;gap:5px;flex-shrink:0;}
  .pcard-actions>svg{margin-top:0;}
  .pcard-remove{width:24px;height:24px;border:1px solid transparent;border-radius:7px;background:transparent;color:var(--muted);font-size:20px;line-height:18px;display:grid;place-items:center;cursor:pointer;padding:0;transition:.15s;}
  .pcard-remove:hover,.pcard-remove:focus-visible{border-color:var(--red);background:rgba(224,84,78,.16);color:#fff;outline:none;}
  .action-btn.pipeline-add{color:var(--green);background:rgba(57,192,122,.10);}
  .action-btn.pipeline-add:hover{border-color:var(--green);background:rgba(57,192,122,.18);color:#fff;}
  body.pipeline-list-view th.actions,body.pipeline-list-view td.actions{min-width:285px;}
</style>
""";

        private const string Script = """
<script id="pipeline-visibility-script">
/* Pipeline görünürlüğü müşteri kaydından bağımsızdır. Eksik alanlar geriye uyumluluk için görünür kabul edilir. */
const isInPipeline = lead => lead && lead.inPipeline !== false;

viewPipeline = function(){
  const cols=STAGES.map(s=>{
    const items=S.leads.filter(l=>isInPipeline(l)&&l.stage===s.id);
    const cards=items.map(l=>`<div class="pcard" draggable="true" data-card="${l.id}">
      <div class="pcard-top"><b>${esc(l.company)}</b><span class="pcard-actions"><button type="button" class="pcard-remove" data-pipeline-remove="${l.id}" title="Pipeline'dan kaldır" aria-label="${esc(l.company)} müşterisini pipeline'dan kaldır">×</button>${ICON.grip}</span></div>
      <div class="pcard-svc">${statusTag(l.status)}${is2026(l)?`<span class="tag" style="background:#ffffff0d;color:var(--body)">${monthLabel(monthKey(l))}</span>`:""}</div>
      <div class="pcard-foot"><b style="color:${s.color}">${fmtValue(l)}</b><span style="display:flex;gap:3px">${ownerChips(ownersOf(l))}</span></div>
    </div>`).join("");
    return `<div class="col" data-col="${s.id}"><div class="col-head"><i style="background:${s.color}"></i><b>${s.label}</b><span class="cnt">${items.length}</span></div><div class="col-total">${fmtTRY(sum(items))}</div><div class="col-list">${cards}</div></div>`;
  }).join("");
  return `<div class="kanban" id="kanban">${cols}</div>`;
};

leadRows = function(list){
  return list.map(l=>`<tr>
    <td><div class="co">${esc(l.company)}</div>${l.contact?`<div class="co-sub">${esc(l.contact)}</div>`:""}</td>
    <td>${statusTag(l.status)}</td><td>${monthCell(l)}</td><td>${tempTag(l.temp)}</td>
    <td class="val">${fmtValue(l)}</td><td>${pill(l.stage)}</td><td>${ownerCell(ownersOf(l))}</td>
    <td class="actions">${S.view==="leads"&&!isInPipeline(l)?`<button type="button" class="action-btn pipeline-add" data-pipeline-add="${l.id}" title="Pipeline'a yeniden ekle"><span>＋ Pipeline'a Ekle</span></button>`:""}<button class="action-btn" data-edit="${l.id}" title="Düzenle">${ICON.pencil}<span>Düzenle</span></button><button class="action-btn danger" data-del="${l.id}" title="Sil">${ICON.trash}<span>Sil</span></button></td>
  </tr>`).join("");
};

openForm = function(lead){
  const ow=lead?ownersOf(lead):[OWNERS[0]];
  const f=lead||{company:"",contact:"",email:"",source:SOURCES[0],status:STATUS[0],temp:"sicak",value:"",stage:"new",date:new Date().toISOString().slice(0,10),note:"",inPipeline:true};
  const o1=ow[0]||OWNERS[0], o2=ow[1]||"";
  const sel=(arr,v)=>arr.map(o=>`<option ${o===v?"selected":""}>${o}</option>`).join("");
  const stSel=STAGES.map(s=>`<option value="${s.id}" ${f.stage===s.id?"selected":""}>${s.label}</option>`).join("");
  const tempSel=`<option value="sicak" ${f.temp==="sicak"?"selected":""}>Sıcak</option><option value="soguk" ${f.temp==="soguk"?"selected":""}>Soğuk</option>`;
  const o2opts=`<option value="">— yok —</option>`+OWNERS.map(o=>`<option ${o===o2?"selected":""}>${o}</option>`).join("");
  document.getElementById("modal-root").innerHTML=`<div class="overlay" id="ov"><div class="modal" id="md">
    <div class="modal-head"><h3>${lead?"Fırsatı düzenle":"Yeni fırsat"}</h3><button class="icon-btn" id="mclose">${ICON.x}</button></div>
    <div class="form">
      <div class="field full"><label>Firma / Müşteri adı</label><input id="f-company" value="${esc(f.company)}" placeholder="Örn. Ege Yapı İnşaat"></div>
      <div class="field"><label>İlgili kişi</label><input id="f-contact" value="${esc(f.contact)}" placeholder="Ad Soyad / Departman"></div>
      <div class="field"><label>E-posta</label><input id="f-email" value="${esc(f.email)}" placeholder="info@firma.com"></div>
      <div class="field"><label>Statü</label><select id="f-status">${sel(STATUS,f.status)}</select><span class="hint">Düzenli: aylık tekrarlayan · Dış: tek seferlik</span></div>
      <div class="field"><label>Müşteri sıcaklığı</label><select id="f-temp">${tempSel}</select></div>
      <div class="field"><label>Sorumlu</label><select id="f-owner1">${sel(OWNERS,o1)}</select></div>
      <div class="field"><label>2. sorumlu (opsiyonel)</label><select id="f-owner2">${o2opts}</select><span class="hint">İki kişide kredi yarıya bölünür</span></div>
      <div class="field"><label>Kaynak</label><select id="f-source">${sel(SOURCES,f.source)}</select></div>
      <div class="field"><label id="lbl-value">Tahmini değer (₺)</label><input id="f-value" type="number" value="${f.value}" placeholder="0"></div>
      <div class="field"><label>Aşama</label><select id="f-stage">${stSel}</select></div>
      <div class="field"><label>Satış ayı</label><input id="f-date" type="month" value="${monthInputValue(f.date)}"><span class="hint">Tablodaki Ay kolonu bu alandan hesaplanır.</span></div>
      <div class="field full"><label>Not</label><textarea id="f-note" placeholder="Görüşme notu, teklif detayı…">${esc(f.note)}</textarea></div>
    </div>
    <div class="modal-foot"><button class="btn-ghost" id="mcancel">Vazgeç</button><button class="btn-primary" id="msave">${lead?"Değişiklikleri kaydet":"Fırsatı ekle"}</button></div>
  </div></div>`;
  const close=()=>document.getElementById("modal-root").innerHTML="";
  const syncLabel=()=>{document.getElementById("lbl-value").textContent=document.getElementById("f-status").value==="Düzenli İş"?"Aylık değer (₺/ay)":"Toplam değer (₺)";};
  syncLabel();document.getElementById("f-status").onchange=syncLabel;
  document.getElementById("ov").onclick=e=>{if(e.target.id==="ov")close();};
  document.getElementById("mclose").onclick=close;document.getElementById("mcancel").onclick=close;
  document.getElementById("f-company").focus();
  document.getElementById("msave").onclick=()=>{
    const g=id=>document.getElementById(id).value;
    const company=g("f-company").trim();
    if(!company){document.getElementById("f-company").style.borderColor="#E0544E";return;}
    const owners=[g("f-owner1")]; const o2v=g("f-owner2"); if(o2v&&o2v!==owners[0])owners.push(o2v);
    const obj={id:f.id||uid(),company,contact:g("f-contact"),email:g("f-email"),source:g("f-source"),status:g("f-status"),temp:g("f-temp"),value:Number(g("f-value"))||0,owners,stage:g("f-stage"),date:monthToDate(g("f-date")),note:g("f-note"),inPipeline:lead?isInPipeline(lead):true};
    const i=S.leads.findIndex(l=>l.id===obj.id);
    if(i>=0)S.leads[i]=obj;else S.leads.unshift(obj);
    close();commit();
  };
};

bindRowActions = function(){
  document.querySelectorAll("[data-edit]").forEach(b=>b.onclick=()=>openForm(S.leads.find(l=>l.id===b.dataset.edit)));
  document.querySelectorAll("[data-del]").forEach(b=>b.onclick=()=>confirmDelete(S.leads.find(l=>l.id===b.dataset.del)));
  document.querySelectorAll("[data-pipeline-add]").forEach(b=>b.onclick=()=>{const lead=S.leads.find(l=>l.id===b.dataset.pipelineAdd);if(lead){lead.inPipeline=true;commit();}});
};

bindKanban = function(){
  document.querySelectorAll("[data-pipeline-remove]").forEach(button=>{
    button.addEventListener("pointerdown",e=>e.stopPropagation());
    button.addEventListener("dragstart",e=>{e.preventDefault();e.stopPropagation();});
    button.onclick=e=>{e.preventDefault();e.stopPropagation();const lead=S.leads.find(l=>l.id===button.dataset.pipelineRemove);if(lead){lead.inPipeline=false;commit();}};
  });
  document.querySelectorAll(".pcard").forEach(card=>{
    card.addEventListener("dragstart",e=>{if(e.target.closest("[data-pipeline-remove]")){e.preventDefault();return;}dragId=card.dataset.card;});
    card.addEventListener("dragend",()=>{dragId=null;document.querySelectorAll(".col").forEach(c=>c.style.removeProperty("background"));document.querySelectorAll(".col").forEach(c=>c.style.borderColor="var(--line)");});
    card.addEventListener("click",e=>{if(!e.defaultPrevented&&!e.target.closest("[data-pipeline-remove]"))openForm(S.leads.find(l=>l.id===card.dataset.card));});
  });
  document.querySelectorAll(".col").forEach(col=>{const stage=stageOf(col.dataset.col);
    col.addEventListener("dragover",e=>{e.preventDefault();col.style.background=stage.color+"14";col.style.borderColor=stage.color;});
    col.addEventListener("dragleave",()=>{col.style.removeProperty("background");col.style.borderColor="var(--line)";});
    col.addEventListener("drop",()=>{col.style.removeProperty("background");col.style.borderColor="var(--line)";if(dragId){const l=S.leads.find(x=>x.id===dragId);if(l&&isInPipeline(l)&&l.stage!==col.dataset.col){l.stage=col.dataset.col;commit();}}dragId=null;});
  });
};

const baseRenderForPipelineVisibility=render;
render=function(){
  document.body.classList.toggle("pipeline-list-view",S.view==="leads");
  baseRenderForPipelineVisibility();
};
TITLES.pipeline[1]="Aşamalar arası sürükle-bırak · × ile yalnızca pipeline'dan kaldır";
</script>
""";

        public static string Apply(string html)
        {
            ArgumentNullException.ThrowIfNull(html);

            if (!html.Contains("</head>", StringComparison.Ordinal) ||
                !html.Contains("</body>", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Marketing HTML şablonunda head/body kapanış etiketleri bulunamadı.");
            }

            return html
                .Replace("</head>", Styles + "</head>", StringComparison.Ordinal)
                .Replace("</body>", Script + "</body>", StringComparison.Ordinal);
        }
    }
}
