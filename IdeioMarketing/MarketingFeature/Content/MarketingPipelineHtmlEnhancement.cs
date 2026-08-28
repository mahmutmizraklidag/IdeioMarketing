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
  .pipeline-filters{align-items:flex-end;margin-bottom:16px;}
  .pipeline-filter-field{display:flex;min-width:180px;flex:1;flex-direction:column;gap:6px;}
  .pipeline-filter-field>span{color:var(--muted);font-size:11px;font-weight:700;letter-spacing:.04em;text-transform:uppercase;}
  .pipeline-filter-field select{width:100%;}
  .pipeline-owner-chips{display:flex;flex-shrink:0;flex-wrap:wrap;justify-content:flex-end;gap:4px;max-width:104px;}
  .pipeline-owner-chips .chip{flex:0 0 22px;}
  .contract-field.is-hidden{display:none;}
  .contract-check{display:flex;align-items:center;gap:10px;width:max-content;cursor:pointer;color:var(--ink);font-size:14px;font-weight:700;}
  .contract-check input{width:18px;height:18px;margin:0;accent-color:var(--green);cursor:pointer;}
  .contract-badge{display:inline-flex;align-items:center;gap:5px;width:max-content;padding:3px 8px;border:1px solid transparent;border-radius:999px;font-size:11px;font-weight:800;white-space:nowrap;}
  .contract-badge.has-contract{border-color:rgba(57,192,122,.28);background:rgba(57,192,122,.13);color:var(--green);}
  .contract-badge.no-contract{border-color:rgba(230,169,60,.26);background:rgba(230,169,60,.11);color:#E6A93C;}
  .contract-badge.is-deferred{border-color:rgba(79,160,230,.28);background:rgba(79,160,230,.12);color:#4FA0E6;}
  .contract-summary{display:flex;flex-direction:column;align-items:flex-start;gap:5px;}
  .contract-dates{color:var(--muted);font-size:11px;line-height:1.35;white-space:nowrap;}
  .contract-date-error{display:none;color:var(--red);font-size:12px;}
  .contract-date-error.is-visible{display:block;}
  .contract-prompt-modal{max-width:560px;}
  .contract-prompt-copy{margin:-2px 0 18px;color:var(--muted);font-size:13px;line-height:1.55;}
  .contract-prompt-form{display:grid;grid-template-columns:1fr 1fr;gap:14px;}
  .contract-prompt-form .full{grid-column:1/-1;}
  .loss-reason-field.is-hidden{display:none;}
  .stage-summary{display:flex;flex-direction:column;align-items:flex-start;gap:5px;}
  .loss-reason-badge{display:inline-flex;max-width:220px;padding:3px 8px;border:1px solid rgba(224,84,78,.25);border-radius:8px;background:rgba(224,84,78,.10);color:#E98B87;font-size:10px;font-weight:700;line-height:1.35;}
  .seg button.team-loss-mode.on{border-color:rgba(224,84,78,.55);background:rgba(224,84,78,.18);color:#F29A96;}
  body .seg-mode{max-width:100%;overflow-x:auto;}
  body .seg-mode button{flex:0 0 auto;}
  .team-loss-reason-list{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:10px;margin-top:14px;}
  .team-loss-reason-item{display:flex;align-items:center;justify-content:space-between;gap:12px;padding:11px 12px;border:1px solid var(--line);border-radius:10px;background:var(--surface2);}
  .team-loss-reason-item span{color:var(--body);font-size:12px;}
  .team-loss-reason-item b{color:#E98B87;font-family:'Space Grotesk',sans-serif;font-size:17px;}
  .contract-expiry-kpi{width:100%;font:inherit;text-align:left;color:inherit;cursor:pointer;transition:border-color .15s,transform .15s,background .15s;}
  .contract-expiry-kpi:hover,.contract-expiry-kpi:focus-visible{border-color:#E6A93C;background:rgba(230,169,60,.06);transform:translateY(-1px);outline:none;}
  .contract-expiry-kpi::after{background:linear-gradient(90deg,transparent,#E6A93C,transparent);opacity:.65;}
  .contract-expiry-kpi.has-expiring{border-color:rgba(224,84,78,.62);background:linear-gradient(145deg,rgba(224,84,78,.13),var(--surface) 68%);box-shadow:0 0 0 1px rgba(224,84,78,.10),0 10px 30px rgba(224,84,78,.10);}
  .contract-expiry-kpi.has-expiring::after{background:linear-gradient(90deg,transparent,#E0544E,transparent);opacity:1;}
  .contract-alarm-icon{position:relative;background:rgba(224,84,78,.18)!important;color:#F17470!important;}
  .contract-alarm-dot{position:absolute;top:10px;right:10px;width:9px;height:9px;border:2px solid var(--surface);border-radius:50%;background:#E0544E;box-shadow:0 0 0 0 rgba(224,84,78,.55);animation:contract-alarm-pulse 1.8s ease-out infinite;}
  .contract-expiry-kpi.has-expiring .kpi-val{color:#F17470;}
  @keyframes contract-alarm-pulse{0%{box-shadow:0 0 0 0 rgba(224,84,78,.55)}70%{box-shadow:0 0 0 9px rgba(224,84,78,0)}100%{box-shadow:0 0 0 0 rgba(224,84,78,0)}}
  @media(prefers-reduced-motion:reduce){.contract-alarm-dot{animation:none;}}
  .contract-expiry-modal{max-width:840px;}
  .contract-expiry-day{display:inline-flex;padding:4px 8px;border-radius:999px;background:rgba(230,169,60,.13);color:#E6A93C;font-size:11px;font-weight:800;white-space:nowrap;}
  body.pipeline-page .content{min-width:0;}
  .pipeline-board-wrap{min-width:0;position:relative;}
  .pipeline-board-toolbar{display:flex;align-items:center;justify-content:space-between;gap:12px;margin:0 0 9px;}
  .pipeline-scroll-hint{display:flex;align-items:center;gap:7px;color:var(--muted);font-size:12px;}
  .pipeline-scroll-hint svg{width:16px;height:16px;flex:0 0 16px;}
  .pipeline-scroll-actions{display:flex;align-items:center;gap:6px;flex-shrink:0;}
  .pipeline-scroll-btn{display:grid;place-items:center;width:34px;height:34px;padding:0;border:1px solid var(--line);border-radius:9px;background:var(--surface2);color:var(--ink);cursor:pointer;transition:.15s;}
  .pipeline-scroll-btn:hover:not(:disabled),.pipeline-scroll-btn:focus-visible{border-color:var(--orange);color:#fff;outline:none;}
  .pipeline-scroll-btn:disabled{cursor:default;opacity:.3;}
  .pipeline-scroll-btn svg{width:17px;height:17px;}
  .pipeline-board-wrap .kanban{cursor:grab;overscroll-behavior-x:contain;scrollbar-gutter:stable;scroll-snap-type:x proximity;}
  .pipeline-board-wrap .kanban.is-panning{cursor:grabbing;scroll-snap-type:none;user-select:none;}
  .pipeline-board-wrap .kanban.is-panning *{cursor:grabbing!important;user-select:none;}
  .pipeline-board-wrap .col{scroll-snap-align:start;}
  @media(max-width:1100px){
    .pipeline-filter-field{flex:1 1 calc(50% - 6px);min-width:220px;}
    .pipeline-board-wrap .col{width:286px;}
  }
  @media(max-width:720px){
    .pipeline-filters{display:grid;grid-template-columns:1fr;gap:10px;margin-bottom:14px;}
    .pipeline-filter-field{min-width:0;width:100%;}
    .pipeline-filter-field select{min-height:44px;font-size:16px;}
    .pipeline-board-wrap .kanban{gap:10px;margin:0 -14px;padding:0 14px 18px;scroll-padding-inline:14px;}
    .pipeline-board-wrap .col{box-sizing:border-box;width:min(84vw,320px);padding:9px;}
    .pipeline-board-wrap .pcard{padding:13px;cursor:pointer;}
    .pipeline-board-wrap .pcard-remove{width:32px;height:32px;}
    .pipeline-owner-chips{max-width:132px;}
    .pipeline-scroll-btn{width:40px;height:40px;}
    .contract-prompt-form{grid-template-columns:1fr;}
    .contract-prompt-form .full{grid-column:auto;}
    .team-loss-reason-list{grid-template-columns:1fr;}
  }
  @media(max-width:420px){
    .pipeline-scroll-hint{font-size:11px;}
    .pipeline-board-wrap .col{width:calc(100vw - 44px);}
    .pipeline-board-wrap .pcard-foot{align-items:flex-end;gap:8px;}
  }
  @media(pointer:coarse){.pipeline-board-wrap .pcard{-webkit-user-drag:none;}}
</style>
""";

        private const string Script = """
<script id="pipeline-visibility-script">
/* Pipeline görünürlüğü müşteri kaydından bağımsızdır. Eksik alanlar geriye uyumluluk için görünür kabul edilir. */
if(!STAGES.some(stage=>stage.id==="rejected")){
  STAGES.push({id:"rejected",label:"Reddedildi",color:"#C65F7B"});
}
const isInPipeline = lead => lead && lead.inPipeline !== false;
const LOSS_REASONS=["Müşteri Vazgeçti","Sözleşme Yenilenmedi","Sözleşme Müşteri Tarafından Fesedildi","Sözleşme Firma Tarafından Fes Edildi"];
const lossReasonOptions = selected => `<option value="">Sebep seçin</option>`+LOSS_REASONS.map(reason=>`<option value="${esc(reason)}" ${selected===reason?"selected":""}>${esc(reason)}</option>`).join("");
const lossReasonOf = lead => lead?.lossReason||"Sebep girilmemiş";
const lossReasonBadge = lead => lead?.stage==="lost"?`<span class="loss-reason-badge">${esc(lossReasonOf(lead))}</span>`:"";
const stageCell = lead => `<span class="stage-summary">${pill(lead.stage)}${lossReasonBadge(lead)}</span>`;
const teamPerformanceSeg = () => `<div class="seg seg-mode">${[["duzenli","Düzenli"],["dis","Dış İş"],["toplam","Toplam Satış"]].map(([key,label])=>`<button class="${!S.teamLossMode&&S.salesMode===key?"on":""}" data-team-sales-mode="${key}">${label}</button>`).join("")}<button class="team-loss-mode ${S.teamLossMode?"on":""}" data-team-loss-mode="true">Kaybedilen Müşteriler</button></div>`;

const viewTeamLosses = () => {
  const lost=S.leads.filter(lead=>lead.stage==="lost");
  const reasonDefinitions=[
    {reason:LOSS_REASONS[0],label:"Vazgeçti"},
    {reason:LOSS_REASONS[1],label:"Yenilenmedi"},
    {reason:LOSS_REASONS[2],label:"Müşteri feshi"},
    {reason:LOSS_REASONS[3],label:"Firma feshi"},
    {reason:"Sebep girilmemiş",label:"Girilmemiş"}
  ];
  const reasonColors=["#E0544E","#E6A93C","#C65F7B","#9A7BD4","#7E7C86"];
  const reasonData=reasonDefinitions.map((item,index)=>({reason:item.reason,label:item.label,value:lost.filter(lead=>lossReasonOf(lead)===item.reason).length,color:reasonColors[index],tip:`${item.reason}: ${lost.filter(lead=>lossReasonOf(lead)===item.reason).length} müşteri`}));
  const reasonSummary=reasonData.map(item=>`<div class="team-loss-reason-item"><span>${esc(item.reason)}</span><b>${item.value}</b></div>`).join("");
  const mostCommon=[...reasonData].sort((a,b)=>b.value-a.value)[0]||{label:"—",value:0};
  const ownerData=ownerCountData(lost);
  const knownReasonCount=lost.filter(lead=>lead.lossReason).length;
  const lostValue=sum(lost);
  const rows=[...lost].sort((a,b)=>String(b.date||"").localeCompare(String(a.date||""))||String(a.company).localeCompare(String(b.company),"tr")).map(lead=>`<tr>
    <td><div class="co">${esc(lead.company)}</div>${lead.contact?`<div class="co-sub">${esc(lead.contact)}</div>`:""}</td>
    <td>${lossReasonBadge(lead)}</td>
    <td>${monthCell(lead)}</td>
    <td>${statusTag(lead.status)}</td>
    <td class="val">${fmtValue(lead)}</td>
    <td>${ownerCell(ownersOf(lead))}</td>
    <td class="actions"><button class="action-btn" data-edit="${lead.id}" title="Düzenle">${ICON.pencil}<span>Düzenle</span></button></td>
  </tr>`).join("");

  return `<div style="display:flex;flex-direction:column;gap:20px;animation:fade .25s ease">
    ${teamPerformanceSeg()}
    <div class="kpi-grid">
      ${kpi("churn","Kaybedilen müşteriler",lost.length,"tüm zamanlar","#E0544E")}
      ${kpi("perf","En sık kayıp sebebi",esc(mostCommon.value?mostCommon.reason:"—"),mostCommon.value+" müşteri","#E6A93C")}
      ${kpi("users","Sebebi girilen",knownReasonCount,lost.length+" kaydın "+knownReasonCount+" tanesi","#C65F7B")}
      ${kpi("wallet","Kaybedilen toplam değer",fmtTRY(lostValue),lost.length+" kayıt","#9A7BD4")}
    </div>
    <div class="grid-2">
      <div class="card"><h3>Kaybedilme sebeplerine göre müşteriler</h3>${barChart(reasonData)}<div class="team-loss-reason-list">${reasonSummary}</div></div>
      <div class="card"><h3>Sorumluya göre kaybedilen müşteriler</h3>${donut(ownerData,"Kayıp")}<div class="legend">${legendOf(ownerData)}</div></div>
    </div>
    <div class="card" style="padding:0"><div style="padding:20px 20px 0"><h3 style="margin:0">Hangi müşteri neden kaybedildi?</h3></div>
      <div style="overflow-x:auto"><table><thead><tr><th>Müşteri</th><th>Kaybedilme sebebi</th><th>Ay</th><th>Statü</th><th class="r">Değer</th><th>Sorumlu</th><th class="actions">İşlem</th></tr></thead><tbody>${rows||`<tr><td colspan="7" class="empty">Kaybedilen müşteri bulunmuyor.</td></tr>`}</tbody></table></div>
    </div>
  </div>`;
};

const bindTeamPerformanceModes = () => {
  document.querySelectorAll("[data-team-sales-mode]").forEach(button=>button.onclick=()=>{S.teamLossMode=false;S.salesMode=button.dataset.teamSalesMode;render();});
  const lossButton=document.querySelector("[data-team-loss-mode]");
  if(lossButton)lossButton.onclick=()=>{S.teamLossMode=true;render();};
};
const formatContractDate = value => {
  if(!value)return "";
  const parts=String(value).slice(0,10).split("-");
  return parts.length===3?`${parts[2]}.${parts[1]}.${parts[0]}`:String(value);
};
const contractDateText = lead => {
  const start=formatContractDate(lead?.contractStartDate),end=formatContractDate(lead?.contractEndDate);
  if(start&&end)return `${start} – ${end}`;
  if(start)return `Başlangıç: ${start}`;
  if(end)return `Bitiş: ${end}`;
  return "";
};
const contractBadge = lead => lead?.stage!=="won"
  ? `<span style="color:var(--muted)">—</span>`
  : lead.contractDeferred===true
    ? `<span class="contract-badge is-deferred">Daha sonra girilecek</span>`
    : `<span class="contract-badge ${lead.hasContract===true?"has-contract":"no-contract"}">${lead.hasContract===true?"✓ Sözleşme var":"Sözleşme yok"}</span>`;
const contractCell = lead => {
  const dates=contractDateText(lead);
  return `<span class="contract-summary">${contractBadge(lead)}${dates?`<span class="contract-dates">${esc(dates)}</span>`:""}</span>`;
};
const contractDatesAreValid = (start,end) => !start||!end||end>=start;
const parseLocalContractDate = value => {
  const parts=String(value||"").slice(0,10).split("-").map(Number);
  if(parts.length!==3||parts.some(part=>!Number.isFinite(part)))return null;
  const date=new Date(parts[0],parts[1]-1,parts[2]);
  return date.getFullYear()===parts[0]&&date.getMonth()===parts[1]-1&&date.getDate()===parts[2]?date:null;
};
const contractsExpiringSoon = () => {
  const now=new Date(),today=new Date(now.getFullYear(),now.getMonth(),now.getDate()),limit=new Date(today);
  limit.setMonth(limit.getMonth()+1);
  return S.leads.filter(lead=>lead.stage==="won"&&lead.contractEndDate).map(lead=>{
    const endDate=parseLocalContractDate(lead.contractEndDate);
    return {lead,endDate,days:endDate?Math.ceil((endDate-today)/86400000):-1};
  }).filter(item=>item.endDate&&item.endDate>=today&&item.endDate<=limit).sort((a,b)=>a.endDate-b.endDate||String(a.lead.company).localeCompare(String(b.lead.company),"tr"));
};

const openExpiringContractsPrompt = () => {
  const items=contractsExpiringSoon();
  const rows=items.map(item=>`<tr>
    <td><div class="co">${esc(item.lead.company)}</div>${item.lead.contact?`<div class="co-sub">${esc(item.lead.contact)}</div>`:""}</td>
    <td><b style="color:var(--ink)">${formatContractDate(item.lead.contractEndDate)}</b></td>
    <td><span class="contract-expiry-day">${item.days===0?"Bugün":item.days+" gün"}</span></td>
    <td>${contractBadge(item.lead)}</td>
    <td>${ownerCell(ownersOf(item.lead))}</td>
    <td class="actions"><button class="action-btn" data-edit="${item.lead.id}" title="Düzenle">${ICON.pencil}<span>Düzenle</span></button></td>
  </tr>`).join("");
  const root=document.getElementById("modal-root");
  root.innerHTML=`<div class="overlay" id="contract-expiry-overlay"><div class="modal contract-expiry-modal" role="dialog" aria-modal="true" aria-labelledby="contract-expiry-title">
    <div class="modal-head"><div><h3 id="contract-expiry-title">Sözleşmesi yakında bitecek müşteriler</h3><span class="hint">Bitiş tarihi bugün ile önümüzdeki bir ay arasında olan satışlar</span></div><button class="icon-btn" id="contract-expiry-close" aria-label="Pencereyi kapat">${ICON.x}</button></div>
    <div style="overflow-x:auto"><table><thead><tr><th>Müşteri</th><th>Bitiş tarihi</th><th>Kalan</th><th>Sözleşme</th><th>Sorumlu</th><th class="actions">İşlem</th></tr></thead><tbody>${rows||`<tr><td colspan="6" class="empty">Önümüzdeki bir ay içinde bitecek sözleşme bulunmuyor.</td></tr>`}</tbody></table></div>
    <div class="modal-foot"><button class="btn-ghost" id="contract-expiry-done">Kapat</button></div>
  </div></div>`;
  const close=()=>{root.innerHTML="";};
  document.getElementById("contract-expiry-close").onclick=close;
  document.getElementById("contract-expiry-done").onclick=close;
  document.getElementById("contract-expiry-overlay").onclick=event=>{if(event.target.id==="contract-expiry-overlay")close();};
  bindRowActions();
};

const addDashboardContractExpiryCard = () => {
  const grid=document.querySelector("#content .kpi-grid");
  if(!grid)return;
  const count=contractsExpiringSoon().length;
  grid.insertAdjacentHTML("beforeend",`<button type="button" class="kpi contract-expiry-kpi ${count>0?"has-expiring":""}" id="contractExpiryKpi">${count>0?`<span class="contract-alarm-dot" aria-hidden="true"></span>`:""}<span class="kpi-top"><span class="kpi-label">Sözleşmesi yakında bitecek</span><span class="kpi-ic ${count>0?"contract-alarm-icon":""}" style="background:#E6A93C22;color:#E6A93C"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.3 3.7 2.6 17a2 2 0 0 0 1.7 3h15.4a2 2 0 0 0 1.7-3L13.7 3.7a2 2 0 0 0-3.4 0Z"/><path d="M12 9v4M12 17h.01"/></svg></span></span><span class="kpi-val">${count}</span><span class="kpi-sub">Bitişine 1 ay veya daha az kalan müşteri · listeyi aç</span></button>`);
  document.getElementById("contractExpiryKpi").onclick=openExpiringContractsPrompt;
};
const pipelineOwnersOf = lead => {
  const ownerValues=Array.isArray(lead?.owners)
    ? [...lead.owners]
    : (typeof lead?.owners==="string" ? lead.owners.split(/[|,+]/) : []);
  if(lead?.owner)ownerValues.push(lead.owner);
  if(lead?.owner2)ownerValues.push(lead.owner2);
  return [...new Set(ownerValues.map(owner=>String(owner).trim()).filter(Boolean))];
};
const pipelineOwnerChips = lead => pipelineOwnersOf(lead).map(owner=>`<span class="chip" title="${esc(owner)}" aria-label="${esc(owner)}" style="background:${ownerColor(owner)}2A;color:${ownerColor(owner)}">${esc(owner[0]||"?")}</span>`).join("");
const pipelineFilters = {owner:"all",status:"all",month:"all"};
const pipelineFilterOwners = () => [...new Set([...OWNERS,...S.leads.filter(isInPipeline).flatMap(pipelineOwnersOf)])].filter(Boolean);
const pipelineFilterStatuses = () => [...new Set([...STATUS,...S.leads.filter(isInPipeline).map(l=>l.status)])].filter(Boolean);
const pipelineFilterMonths = () => [...new Set(S.leads.filter(isInPipeline).map(monthKey).filter(Boolean))].sort().reverse();
const pipelineFilteredLeads = () => S.leads.filter(l =>
  isInPipeline(l) &&
  (pipelineFilters.owner === "all" || pipelineOwnersOf(l).includes(pipelineFilters.owner)) &&
  (pipelineFilters.status === "all" || l.status === pipelineFilters.status) &&
  (pipelineFilters.month === "all" || monthKey(l) === pipelineFilters.month)
);

viewPipeline = function(){
  const filtered=pipelineFilteredLeads();
  const ownerOpts=`<option value="all">Tüm sorumlular</option>`+pipelineFilterOwners().map(o=>`<option value="${esc(o)}" ${pipelineFilters.owner===o?"selected":""}>${esc(o)}</option>`).join("");
  const statusOpts=`<option value="all">Tüm statüler</option>`+pipelineFilterStatuses().map(s=>`<option value="${esc(s)}" ${pipelineFilters.status===s?"selected":""}>${esc(s)}</option>`).join("");
  const monthOpts=`<option value="all">Tüm aylar ve yıllar</option>`+pipelineFilterMonths().map(k=>`<option value="${k}" ${pipelineFilters.month===k?"selected":""}>${monthLabelAny(k)}</option>`).join("");
  const cols=STAGES.map(s=>{
    const items=filtered.filter(l=>l.stage===s.id);
    const cards=items.map(l=>`<div class="pcard" draggable="true" data-card="${l.id}">
      <div class="pcard-top"><b>${esc(l.company)}</b><span class="pcard-actions"><button type="button" class="pcard-remove" data-pipeline-remove="${l.id}" title="Pipeline'dan kaldır" aria-label="${esc(l.company)} müşterisini pipeline'dan kaldır">×</button>${ICON.grip}</span></div>
      <div class="pcard-svc">${statusTag(l.status)}${is2026(l)?`<span class="tag" style="background:#ffffff0d;color:var(--body)">${monthLabel(monthKey(l))}</span>`:""}${l.stage==="won"?contractCell(l):""}${l.stage==="lost"?lossReasonBadge(l):""}</div>
      <div class="pcard-foot"><b style="color:${s.color}">${fmtValue(l)}</b><span class="pipeline-owner-chips">${pipelineOwnerChips(l)}</span></div>
    </div>`).join("");
    return `<div class="col" data-col="${s.id}"><div class="col-head"><i style="background:${s.color}"></i><b>${s.label}</b><span class="cnt">${items.length}</span></div><div class="col-total">${fmtTRY(sum(items))}</div><div class="col-list">${cards}</div></div>`;
  }).join("");
  return `<div class="filters pipeline-filters" aria-label="Pipeline filtreleri">
    <label class="pipeline-filter-field"><span>Sorumlu</span><select class="flt" id="pipelineOwnerFilter">${ownerOpts}</select></label>
    <label class="pipeline-filter-field"><span>Statü</span><select class="flt" id="pipelineStatusFilter">${statusOpts}</select></label>
    <label class="pipeline-filter-field"><span>Ay ve yıl</span><select class="flt" id="pipelineMonthFilter">${monthOpts}</select></label>
  </div><div class="pipeline-board-wrap"><div class="pipeline-board-toolbar"><div class="pipeline-scroll-hint" aria-hidden="true"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="M5 12h14M8 9l-3 3 3 3M16 9l3 3-3 3"/></svg>Aşamalar arasında yatay ilerleyin</div><div class="pipeline-scroll-actions"><button type="button" class="pipeline-scroll-btn" id="pipelineScrollPrev" aria-label="Önceki aşamaları göster"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 18-6-6 6-6"/></svg></button><button type="button" class="pipeline-scroll-btn" id="pipelineScrollNext" aria-label="Sonraki aşamaları göster"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg></button></div></div><div class="kanban" id="kanban" role="region" aria-label="Fırsat aşamaları" tabindex="0">${cols}</div></div>`;
};

const bindPipelineScroller=()=>{
  const board=document.getElementById("kanban");
  const previous=document.getElementById("pipelineScrollPrev");
  const next=document.getElementById("pipelineScrollNext");
  if(!board||!previous||!next)return;

  const update=()=>{
    const maxScroll=Math.max(0,board.scrollWidth-board.clientWidth);
    previous.disabled=board.scrollLeft<=2;
    next.disabled=board.scrollLeft>=maxScroll-2;
  };
  const distance=()=>Math.max(282,Math.min(board.clientWidth*.8,720));
  previous.onclick=()=>board.scrollBy({left:-distance(),behavior:"smooth"});
  next.onclick=()=>board.scrollBy({left:distance(),behavior:"smooth"});
  board.addEventListener("scroll",update,{passive:true});

  let pan=null;
  board.addEventListener("pointerdown",event=>{
    if(event.pointerType!=="mouse"||event.button!==0||event.target.closest(".pcard,button,a,input,select,textarea"))return;
    pan={pointerId:event.pointerId,startX:event.clientX,startScrollLeft:board.scrollLeft};
    board.setPointerCapture(event.pointerId);
    board.classList.add("is-panning");
    event.preventDefault();
  });
  board.addEventListener("pointermove",event=>{
    if(!pan||event.pointerId!==pan.pointerId)return;
    board.scrollLeft=pan.startScrollLeft+(pan.startX-event.clientX);
    event.preventDefault();
  });
  const stopPanning=event=>{
    if(!pan||event.pointerId!==pan.pointerId)return;
    if(board.hasPointerCapture(event.pointerId))board.releasePointerCapture(event.pointerId);
    pan=null;
    board.classList.remove("is-panning");
    update();
  };
  board.addEventListener("pointerup",stopPanning);
  board.addEventListener("pointercancel",stopPanning);
  requestAnimationFrame(update);
};

leadRows = function(list){
  return list.map(l=>`<tr>
    <td><div class="co">${esc(l.company)}</div>${l.contact?`<div class="co-sub">${esc(l.contact)}</div>`:""}</td>
    <td>${statusTag(l.status)}</td><td>${monthCell(l)}</td><td>${tempTag(l.temp)}</td>
    <td class="val">${fmtValue(l)}</td><td>${stageCell(l)}</td><td>${contractCell(l)}</td><td>${ownerCell(ownersOf(l))}</td>
    <td class="actions">${S.view==="leads"&&!isInPipeline(l)?`<button type="button" class="action-btn pipeline-add" data-pipeline-add="${l.id}" title="Pipeline'a yeniden ekle"><span>＋ Pipeline'a Ekle</span></button>`:""}<button class="action-btn" data-edit="${l.id}" title="Düzenle">${ICON.pencil}<span>Düzenle</span></button><button class="action-btn danger" data-del="${l.id}" title="Sil">${ICON.trash}<span>Sil</span></button></td>
  </tr>`).join("");
};

leadTable = function(list,emptyMsg){
  return `<div class="table-wrap"><div style="overflow-x:auto"><table>
    <thead><tr><th>Firma</th><th>Statü</th><th>Ay</th><th>Sıcaklık</th><th class="r">Değer</th><th>Aşama</th><th>Sözleşme</th><th>Sorumlu(lar)</th><th class="actions">İşlem</th></tr></thead>
    <tbody>${leadRows(list)||`<tr><td colspan="9" class="empty">${emptyMsg||"Kayıt yok."}</td></tr>`}</tbody></table></div></div>`;
};

openForm = function(lead){
  const ow=lead?ownersOf(lead):[OWNERS[0]];
  const f=lead||{company:"",contact:"",email:"",source:SOURCES[0],status:STATUS[0],temp:"sicak",value:"",stage:"new",date:new Date().toISOString().slice(0,10),note:"",inPipeline:true,hasContract:false,contractStartDate:"",contractEndDate:"",contractDeferred:false,lossReason:""};
  const o1=ow[0]||OWNERS[0], o2=ow[1]||"";
  const sel=(arr,v)=>arr.map(o=>`<option ${o===v?"selected":""}>${o}</option>`).join("");
  const stSel=STAGES.filter(s=>s.id!=="rejected").map(s=>`<option value="${s.id}" ${f.stage===s.id?"selected":""}>${s.label}</option>`).join("")+`<option value="rejected" ${f.stage==="rejected"?"selected":""}>Reddedildi</option>`;
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
      <div class="field full contract-field ${f.stage==="won"?"":"is-hidden"}" data-contract-field><label class="contract-check"><input id="f-has-contract" type="checkbox" ${f.hasContract===true?"checked":""}><span>Sözleşmesi var mı?</span></label><span class="hint">Zorunlu değildir; daha sonra da güncelleyebilirsiniz.</span></div>
      <div class="field contract-field ${f.stage==="won"?"":"is-hidden"}" data-contract-field><label>Sözleşme başlangıç tarihi</label><input id="f-contract-start" type="date" value="${esc(f.contractStartDate||"")}"></div>
      <div class="field contract-field ${f.stage==="won"?"":"is-hidden"}" data-contract-field><label>Sözleşme bitiş tarihi</label><input id="f-contract-end" type="date" value="${esc(f.contractEndDate||"")}"><span class="contract-date-error" id="f-contract-date-error">Bitiş tarihi başlangıç tarihinden önce olamaz.</span></div>
      <div class="field full loss-reason-field ${f.stage==="lost"?"":"is-hidden"}" data-loss-reason-field><label>Kaybedilme sebebi</label><select id="f-loss-reason">${lossReasonOptions(f.lossReason||"")}</select><span class="contract-date-error" id="f-loss-reason-error">Lütfen bir kaybedilme sebebi seçin.</span></div>
      <div class="field full"><label>Not</label><textarea id="f-note" placeholder="Görüşme notu, teklif detayı…">${esc(f.note)}</textarea></div>
    </div>
    <div class="modal-foot"><button class="btn-ghost" id="mcancel">Vazgeç</button><button class="btn-primary" id="msave">${lead?"Değişiklikleri kaydet":"Fırsatı ekle"}</button></div>
  </div></div>`;
  const close=()=>document.getElementById("modal-root").innerHTML="";
  const syncLabel=()=>{document.getElementById("lbl-value").textContent=document.getElementById("f-status").value==="Düzenli İş"?"Aylık değer (₺/ay)":"Toplam değer (₺)";};
  const syncStageSpecificFields=()=>{const stage=document.getElementById("f-stage").value;document.querySelectorAll("[data-contract-field]").forEach(field=>field.classList.toggle("is-hidden",stage!=="won"));document.querySelectorAll("[data-loss-reason-field]").forEach(field=>field.classList.toggle("is-hidden",stage!=="lost"));};
  syncLabel();document.getElementById("f-status").onchange=syncLabel;
  syncStageSpecificFields();document.getElementById("f-stage").onchange=syncStageSpecificFields;
  document.getElementById("ov").onclick=e=>{if(e.target.id==="ov")close();};
  document.getElementById("mclose").onclick=close;document.getElementById("mcancel").onclick=close;
  document.getElementById("f-company").focus();
  document.getElementById("msave").onclick=async()=>{
    const g=id=>document.getElementById(id).value;
    const company=g("f-company").trim();
    if(!company){document.getElementById("f-company").style.borderColor="#E0544E";return;}
    const owners=[g("f-owner1")]; const o2v=g("f-owner2"); if(o2v&&o2v!==owners[0])owners.push(o2v);
    const stage=g("f-stage");
    const hasContract=stage==="won"&&document.getElementById("f-has-contract").checked;
    const contractStartDate=stage==="won"?g("f-contract-start"):"",contractEndDate=stage==="won"?g("f-contract-end"):"";
    const dateError=document.getElementById("f-contract-date-error");
    dateError.classList.toggle("is-visible",!contractDatesAreValid(contractStartDate,contractEndDate));
    if(!contractDatesAreValid(contractStartDate,contractEndDate))return;
    const lossReason=stage==="lost"?g("f-loss-reason"):"";
    const lossReasonError=document.getElementById("f-loss-reason-error");
    lossReasonError.classList.toggle("is-visible",stage==="lost"&&!lossReason);
    if(stage==="lost"&&!lossReason)return;
    const obj={id:f.id||uid(),company,contact:g("f-contact"),email:g("f-email"),source:g("f-source"),status:g("f-status"),temp:g("f-temp"),value:Number(g("f-value"))||0,owners,stage,hasContract,contractStartDate,contractEndDate,contractDeferred:false,lossReason,date:monthToDate(g("f-date")),note:g("f-note"),inPipeline:lead?isInPipeline(lead):true};
    const i=S.leads.findIndex(l=>l.id===obj.id);
    const previous=i>=0?S.leads[i]:null;
    if(i>=0)S.leads[i]=obj;else S.leads.unshift(obj);
    const saveButton=document.getElementById("msave");
    saveButton.disabled=true;saveButton.textContent="Kaydediliyor…";
    try{
      await commit();
      close();
    }catch(error){
      if(i>=0)S.leads[i]=previous;else S.leads=S.leads.filter(l=>l.id!==obj.id);
      saveButton.disabled=false;saveButton.textContent=lead?"Değişiklikleri kaydet":"Fırsatı ekle";
      let message=document.getElementById("save-error");
      if(!message){message=document.createElement("div");message.id="save-error";message.style.cssText="color:#E0544E;font-size:13px;margin-right:auto";saveButton.parentElement.prepend(message);}
      message.textContent="Kayıt veritabanına yazılamadı. Lütfen tekrar deneyin.";
      console.error("Marketing kaydı veritabanına yazılamadı.",error);
    }
  };
};

bindRowActions = function(){
  document.querySelectorAll("[data-edit]").forEach(b=>b.onclick=()=>openForm(S.leads.find(l=>l.id===b.dataset.edit)));
  document.querySelectorAll("[data-del]").forEach(b=>b.onclick=()=>confirmDelete(S.leads.find(l=>l.id===b.dataset.del)));
  document.querySelectorAll("[data-pipeline-add]").forEach(b=>b.onclick=()=>{const lead=S.leads.find(l=>l.id===b.dataset.pipelineAdd);if(lead){lead.inPipeline=true;commit();}});
};

const openWonContractPrompt = lead => {
  const root=document.getElementById("modal-root");
  root.innerHTML=`<div class="overlay" id="contract-prompt-overlay"><div class="modal contract-prompt-modal" role="dialog" aria-modal="true" aria-labelledby="contract-prompt-title">
    <div class="modal-head"><h3 id="contract-prompt-title">Satış tamamlandı</h3><button class="icon-btn" id="contract-prompt-close" aria-label="Pencereyi kapat">${ICON.x}</button></div>
    <p class="contract-prompt-copy"><b>${esc(lead.company)}</b> için sözleşme bilgilerini şimdi ekleyebilirsiniz. Bu alanlar zorunlu değildir.</p>
    <div class="contract-prompt-form">
      <div class="field full"><label class="contract-check"><input id="contract-prompt-has-contract" type="checkbox"><span>Sözleşmesi var mı?</span></label></div>
      <div class="field"><label>Sözleşme başlangıç tarihi</label><input id="contract-prompt-start" type="date"></div>
      <div class="field"><label>Sözleşme bitiş tarihi</label><input id="contract-prompt-end" type="date"><span class="contract-date-error" id="contract-prompt-date-error">Bitiş tarihi başlangıç tarihinden önce olamaz.</span></div>
    </div>
    <div class="modal-foot"><span class="contract-date-error" id="contract-prompt-save-error">Kayıt veritabanına yazılamadı. Lütfen tekrar deneyin.</span><button class="btn-ghost" id="contract-prompt-later">Daha sonra gireceğim</button><button class="btn-primary" id="contract-prompt-save">Satışı tamamla</button></div>
  </div></div>`;

  const close=()=>{root.innerHTML="";};
  const checkbox=document.getElementById("contract-prompt-has-contract");
  const startInput=document.getElementById("contract-prompt-start");
  const endInput=document.getElementById("contract-prompt-end");
  document.getElementById("contract-prompt-close").onclick=close;
  document.getElementById("contract-prompt-overlay").onclick=event=>{if(event.target.id==="contract-prompt-overlay")close();};

  const complete=async deferred=>{
    const hasContract=!deferred&&checkbox.checked;
    const contractStartDate=deferred?"":startInput.value;
    const contractEndDate=deferred?"":endInput.value;
    const dateError=document.getElementById("contract-prompt-date-error");
    dateError.classList.toggle("is-visible",!contractDatesAreValid(contractStartDate,contractEndDate));
    if(!contractDatesAreValid(contractStartDate,contractEndDate))return;

    const previous={stage:lead.stage,hasContract:lead.hasContract,contractStartDate:lead.contractStartDate,contractEndDate:lead.contractEndDate,contractDeferred:lead.contractDeferred,lossReason:lead.lossReason};
    lead.stage="won";
    lead.hasContract=hasContract;
    lead.contractStartDate=contractStartDate;
    lead.contractEndDate=contractEndDate;
    lead.contractDeferred=deferred;
    lead.lossReason="";
    const buttons=root.querySelectorAll("button");
    buttons.forEach(button=>button.disabled=true);
    try{
      await commit();
      close();
    }catch(error){
      Object.assign(lead,previous);
      buttons.forEach(button=>button.disabled=false);
      document.getElementById("contract-prompt-save-error").classList.add("is-visible");
      console.error("Satış tamamlandı bilgileri veritabanına yazılamadı.",error);
    }
  };

  document.getElementById("contract-prompt-later").onclick=()=>complete(true);
  document.getElementById("contract-prompt-save").onclick=()=>complete(false);
  checkbox.focus();
};

const openLostReasonPrompt = lead => {
  const root=document.getElementById("modal-root");
  root.innerHTML=`<div class="overlay" id="loss-prompt-overlay"><div class="modal contract-prompt-modal" role="dialog" aria-modal="true" aria-labelledby="loss-prompt-title">
    <div class="modal-head"><h3 id="loss-prompt-title">Kaybedilme sebebi</h3><button class="icon-btn" id="loss-prompt-close" aria-label="Pencereyi kapat">${ICON.x}</button></div>
    <p class="contract-prompt-copy"><b>${esc(lead.company)}</b> kaydını “Kaybedildi” aşamasına taşımak için bir sebep seçin.</p>
    <div class="contract-prompt-form">
      <div class="field full"><label>Kaybedilme sebebi</label><select id="loss-prompt-reason">${lossReasonOptions("")}</select><span class="contract-date-error" id="loss-prompt-reason-error">Lütfen bir kaybedilme sebebi seçin.</span></div>
    </div>
    <div class="modal-foot"><span class="contract-date-error" id="loss-prompt-save-error">Kayıt veritabanına yazılamadı. Lütfen tekrar deneyin.</span><button class="btn-ghost" id="loss-prompt-cancel">Vazgeç</button><button class="btn-primary" id="loss-prompt-save">Kaybedildi olarak kaydet</button></div>
  </div></div>`;

  const close=()=>{root.innerHTML="";};
  const reasonSelect=document.getElementById("loss-prompt-reason");
  document.getElementById("loss-prompt-close").onclick=close;
  document.getElementById("loss-prompt-cancel").onclick=close;
  document.getElementById("loss-prompt-overlay").onclick=event=>{if(event.target.id==="loss-prompt-overlay")close();};
  document.getElementById("loss-prompt-save").onclick=async()=>{
    const lossReason=reasonSelect.value;
    const reasonError=document.getElementById("loss-prompt-reason-error");
    reasonError.classList.toggle("is-visible",!lossReason);
    if(!lossReason)return;

    const previous={stage:lead.stage,hasContract:lead.hasContract,contractStartDate:lead.contractStartDate,contractEndDate:lead.contractEndDate,contractDeferred:lead.contractDeferred,lossReason:lead.lossReason};
    lead.stage="lost";
    lead.hasContract=false;
    lead.contractStartDate="";
    lead.contractEndDate="";
    lead.contractDeferred=false;
    lead.lossReason=lossReason;
    const buttons=root.querySelectorAll("button");
    buttons.forEach(button=>button.disabled=true);
    try{
      await commit();
      close();
    }catch(error){
      Object.assign(lead,previous);
      buttons.forEach(button=>button.disabled=false);
      document.getElementById("loss-prompt-save-error").classList.add("is-visible");
      console.error("Kaybedilme sebebi veritabanına yazılamadı.",error);
    }
  };
  reasonSelect.focus();
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
    col.addEventListener("drop",()=>{col.style.removeProperty("background");col.style.borderColor="var(--line)";if(dragId){const l=S.leads.find(x=>x.id===dragId);if(l&&isInPipeline(l)&&l.stage!==col.dataset.col){if(col.dataset.col==="won"){openWonContractPrompt(l);}else if(col.dataset.col==="lost"){openLostReasonPrompt(l);}else{l.stage=col.dataset.col;l.hasContract=false;l.contractStartDate="";l.contractEndDate="";l.contractDeferred=false;l.lossReason="";commit();}}}dragId=null;});
  });
};

const baseRenderForPipelineVisibility=render;
render=function(){
  document.body.classList.toggle("pipeline-list-view",S.view==="leads");
  document.body.classList.toggle("pipeline-page",S.view==="pipeline");
  baseRenderForPipelineVisibility();
  if(S.view==="dashboard")addDashboardContractExpiryCard();
  if(S.view==="team"){
    if(S.teamLossMode){
      document.getElementById("content").innerHTML=viewTeamLosses();
      document.getElementById("page-sub").textContent=S.leads.filter(lead=>lead.stage==="lost").length+" kaybedilen müşteri · sebep analizi";
      bindRowActions();
    }else{
      const salesSegment=document.querySelector(".seg-mode");
      if(salesSegment)salesSegment.outerHTML=teamPerformanceSeg();
    }
    bindTeamPerformanceModes();
  }
  if(S.view==="pipeline"){
    document.getElementById("page-sub").textContent=pipelineFilteredLeads().length+" müşteri";
    document.getElementById("pipelineOwnerFilter").onchange=e=>{pipelineFilters.owner=e.target.value;render();};
    document.getElementById("pipelineStatusFilter").onchange=e=>{pipelineFilters.status=e.target.value;render();};
    document.getElementById("pipelineMonthFilter").onchange=e=>{pipelineFilters.month=e.target.value;render();};
    bindPipelineScroller();
  }
  if(S.view==="leads"){
    const search=document.getElementById("q");
    search.oninput=e=>{S.q=e.target.value;const list=filteredLeads();document.getElementById("page-sub").textContent=list.length+" kayıt";document.querySelector("tbody").innerHTML=leadRows(list)||`<tr><td colspan="9" class="empty">Eşleşen kayıt yok.</td></tr>`;bindRowActions();};
  }
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
