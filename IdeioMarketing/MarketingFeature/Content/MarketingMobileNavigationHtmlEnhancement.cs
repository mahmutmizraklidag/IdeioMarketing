namespace IdeioMarketing.MarketingFeature.Content
{
    /// <summary>
    /// Sıkıştırılmış pazarlama ekranındaki masaüstü menüsünü mobilde açılabilir bir panele dönüştürür.
    /// </summary>
    public static class MarketingMobileNavigationHtmlEnhancement
    {
        private const string SidebarMarker = "<aside class=\"sidebar\">";
        private const string SidebarEndMarker = "</aside>";
        private const string TopbarMarker = "<header class=\"topbar\">";

        private const string Styles = """
<style id="mobile-navigation-styles">
  .mobile-menu-toggle,.mobile-menu-backdrop{display:none;}
  @media(max-width:760px){
    body.mobile-menu-open{overflow:hidden;}
    .sidebar{display:flex;position:fixed;inset:0 auto 0 0;width:min(82vw,300px);height:100vh;height:100dvh;max-height:none;overflow-y:auto;transform:translateX(-105%);transition:transform .22s ease;z-index:120;box-shadow:18px 0 42px rgba(0,0,0,.38);}
    body.mobile-menu-open .sidebar{transform:translateX(0);}
    .mobile-menu-toggle{display:inline-grid;place-items:center;flex:0 0 42px;width:42px;height:42px;padding:0;border:1px solid var(--line);border-radius:10px;background:var(--surface2);color:var(--ink);cursor:pointer;}
    .mobile-menu-toggle:hover,.mobile-menu-toggle:focus-visible{border-color:var(--orange);outline:none;}
    .mobile-menu-toggle svg{width:21px;height:21px;}
    .mobile-menu-backdrop{display:block;position:fixed;inset:0;border:0;background:rgba(0,0,0,.55);opacity:0;visibility:hidden;pointer-events:none;transition:opacity .22s ease,visibility .22s ease;z-index:115;}
    body.mobile-menu-open .mobile-menu-backdrop{opacity:1;visibility:visible;pointer-events:auto;}
    .topbar{gap:12px;padding:14px 16px;}
    .topbar>div{min-width:0;flex:1;}
    .topbar h1{font-size:18px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;}
    .topbar .btn-primary{padding:10px 12px;white-space:nowrap;}
    .content{padding:18px 14px;}
  }
</style>
""";

        private const string ToggleButton = """
<button type="button" class="mobile-menu-toggle" id="mobileMenuToggle" aria-label="Menüyü aç" aria-controls="marketingSidebar" aria-expanded="false">
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="M4 6h16M4 12h16M4 18h16"/></svg>
</button>
""";

        private const string Backdrop = """
<button type="button" class="mobile-menu-backdrop" id="mobileMenuBackdrop" aria-label="Menüyü kapat" tabindex="-1"></button>
""";

        private const string Script = """
<script id="mobile-navigation-script">
(() => {
  const sidebar=document.getElementById("marketingSidebar");
  const toggle=document.getElementById("mobileMenuToggle");
  const backdrop=document.getElementById("mobileMenuBackdrop");
  if(!sidebar||!toggle||!backdrop)return;

  const setOpen=open=>{
    document.body.classList.toggle("mobile-menu-open",open);
    toggle.setAttribute("aria-expanded",String(open));
    toggle.setAttribute("aria-label",open?"Menüyü kapat":"Menüyü aç");
  };

  toggle.addEventListener("click",()=>setOpen(!document.body.classList.contains("mobile-menu-open")));
  backdrop.addEventListener("click",()=>setOpen(false));
  sidebar.addEventListener("click",event=>{
    if(event.target.closest(".nav-btn"))setOpen(false);
  });
  document.addEventListener("keydown",event=>{
    if(event.key==="Escape")setOpen(false);
  });
  window.matchMedia("(min-width:761px)").addEventListener("change",event=>{
    if(event.matches)setOpen(false);
  });
})();
</script>
""";

        public static string Apply(string html)
        {
            ArgumentNullException.ThrowIfNull(html);

            if (!html.Contains("</head>", StringComparison.Ordinal) ||
                !html.Contains("</body>", StringComparison.Ordinal) ||
                !html.Contains(SidebarMarker, StringComparison.Ordinal) ||
                !html.Contains(SidebarEndMarker, StringComparison.Ordinal) ||
                !html.Contains(TopbarMarker, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Marketing HTML şablonunda mobil menü yerleşim noktaları bulunamadı.");
            }

            return html
                .Replace("</head>", Styles + "</head>", StringComparison.Ordinal)
                .Replace(SidebarMarker, "<aside class=\"sidebar\" id=\"marketingSidebar\">", StringComparison.Ordinal)
                .Replace(SidebarEndMarker, SidebarEndMarker + Backdrop, StringComparison.Ordinal)
                .Replace(TopbarMarker, TopbarMarker + ToggleButton, StringComparison.Ordinal)
                .Replace("</body>", Script + "</body>", StringComparison.Ordinal);
        }
    }
}
