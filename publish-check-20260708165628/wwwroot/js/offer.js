document.addEventListener("DOMContentLoaded", function() {
    if (window.__offerWizardInitialized) return;
    window.__offerWizardInitialized = true;

    const wizardData = window.offerWizardData || { categories: [], paymentPlans: [] };
    const offerPdfLogoUrl = window.offerPdfLogoUrl || "/img/logo.png";
    const offerSaveUrl = window.offerSaveUrl || "/Offer/Save";

    const currencyFormatter = new Intl.NumberFormat("tr-TR", {
        style: "currency",
        currency: "TRY",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    const steps = [
        { type: "customer", stepIndex: 0 },
        ...wizardData.categories.map((x, index) => ({
        type: "category",
        stepIndex: index + 1,
        categoryId: x.id,
        categoryName: x.name,
        isPackageMultiSelected: x.isPackageMultiSelected,
        isSubPackageMultiSelected: x.isSubPackageMultiSelected
    })),
        { type: "payment", stepIndex: wizardData.categories.length + 1 },
        { type: "billing", stepIndex: wizardData.categories.length + 2 }
    ];

    const packageLookup = {};
    const subPackageLookup = {};

    wizardData.categories.forEach(category => {
        category.packages.forEach(item => {
            packageLookup[item.id] = {
                ...item,
                categoryId: category.id,
                categoryName: category.name
            };
        });

        category.subPackages.forEach(item => {
            subPackageLookup[item.id] = {
                ...item,
                categoryId: category.id,
                categoryName: category.name
            };
        });
    });

    const state = {
        currentStep: 0,
        selectedPackages: {},
        selectedPackagePieces: {},
        selectedSubPackages: {},
        subPackagePieces: {},
        selectedPaymentPlanId: wizardData.paymentPlans.length ? wizardData.paymentPlans[0].id : null,
        contractDurationMonths: 1,
        enteredDiscountRate: 0,
        selectedDocumentType: "proposal"
    };

    wizardData.categories.forEach(category => {
        state.selectedPackages[category.id] = [];
        state.selectedSubPackages[category.id] = [];

        category.packages.forEach(item => {
            if (item.isPiece) {
                const defaultPiece = parseInt(item.piece || 1, 10);
                state.selectedPackagePieces[item.id] = defaultPiece > 0 ? defaultPiece : 1;
            }
        });

        category.subPackages.forEach(item => {
            if (item.isPiece) {
                const defaultPiece = parseInt(item.piece || 1, 10);
                state.subPackagePieces[item.id] = defaultPiece > 0 ? defaultPiece : 1;
            }
        });
    });

    const prevStepBtn = document.getElementById("prevStepBtn");
    const nextStepBtn = document.getElementById("nextStepBtn");
    const bottomTotalAmount = document.getElementById("bottomTotalAmount");
    const bottomTotalSubtext = document.getElementById("bottomTotalSubtext");
    const contractDurationInput = document.getElementById("contractDurationInput");
    const contractDurationBox = document.getElementById("contractDurationBox");
    const discountRateInput = document.getElementById("discountRateInput");
    const discountWarningText = document.getElementById("discountWarningText");
    const grossTotalBox = document.getElementById("grossTotalBox");
    const discountAmountBox = document.getElementById("discountAmountBox");
    const netTotalBox = document.getElementById("netTotalBox");
    const selectedPlanBox = document.getElementById("selectedPlanBox");
    const paymentScheduleContainer = document.getElementById("paymentScheduleContainer");

    const selectedPackagesJsonInput = document.getElementById("SelectedPackagesJson");
    const selectedPackagePiecesJsonInput = document.getElementById("SelectedPackagePiecesJson");
    const selectedSubPackagesJsonInput = document.getElementById("SelectedSubPackagesJson");
    const selectedSubPackagePiecesJsonInput = document.getElementById("SelectedSubPackagePiecesJson");
    const selectedPaymentPlanIdInput = document.getElementById("SelectedPaymentPlanId");
    const contractDurationMonthsInput = document.getElementById("ContractDurationMonths");
    const appliedDiscountRateInput = document.getElementById("AppliedDiscountRate");
    const selectedDocumentTypeInput = document.getElementById("SelectedDocumentType");

    const customerNameInput = document.getElementById("CustomerName");
    const customerEmailInput = document.getElementById("CustomerEmail");
    const customerPhoneInput = document.getElementById("CustomerPhone");
    const customerTaxOfficeInput = document.getElementById("CustomerTaxOffice");
    const customerTaxNumberInput = document.getElementById("CustomerTaxNumber");
    const customerNotificationAddressInput = document.getElementById("CustomerNotificationAddress");

    const billingDocumentType = document.getElementById("billingDocumentType");
    const billingPreviewArea = document.getElementById("billingPreviewArea");
    const downloadPdfBtn = document.getElementById("downloadPdfBtn");
    const saveFeedbackBox = document.getElementById("saveFeedbackBox");

    const billingSummaryCustomer = document.getElementById("billingSummaryCustomer");
    const billingSummaryDocumentType = document.getElementById("billingSummaryDocumentType");
    const billingSummaryPlan = document.getElementById("billingSummaryPlan");
    const billingSummaryContractDuration = document.getElementById("billingSummaryContractDuration");
    const billingSummaryGross = document.getElementById("billingSummaryGross");
    const billingSummaryDiscount = document.getElementById("billingSummaryDiscount");
    const billingSummaryNet = document.getElementById("billingSummaryNet");

    let isPdfDownloading = false;
    let isSavingOffer = false;

    function round2(value) {
        return Math.round((Number(value) + Number.EPSILON) * 100) / 100;
    }

    function formatCurrency(value) {
        return currencyFormatter.format(round2(value || 0));
    }

    function escapeHtml(text) {
        return String(text ?? "")
   .replaceAll("&", "&amp;")
   .replaceAll("<", "&lt;")
   .replaceAll(">", "&gt;")
   .replaceAll('"', "&quot;")
   .replaceAll("'", "&#039;");
    }

    function escapeAttr(text) {
        return String(text ?? "")
   .replaceAll("&", "&amp;")
   .replaceAll('"', "&quot;")
   .replaceAll("<", "&lt;")
   .replaceAll(">", "&gt;");
    }

    function chunkArray(array, size) {
        if (!Array.isArray(array) || !array.length) {
            return [[]];
        }

        const result = [];
        for (let i = 0; i < array.length; i += size) {
            result.push(array.slice(i, i + size));
        }

        return result.length ? result : [[]];
    }

    async function waitForImages(rootElement) {
        if (!rootElement) return;

        const images = Array.from(rootElement.querySelectorAll("img"));

        await Promise.all(
       images.map(img => {
           if (img.complete) {
               return Promise.resolve();
           }

           return new Promise(resolve => {
               img.onload = () => resolve();
               img.onerror = () => resolve();
           });
       })
   );
    }

    function showSaveFeedback(message, type) {
        if (!saveFeedbackBox) return;

        saveFeedbackBox.className = "save-feedback show " + type;
        saveFeedbackBox.textContent = message;
    }

    function clearSaveFeedback() {
        if (!saveFeedbackBox) return;
        saveFeedbackBox.className = "save-feedback";
        saveFeedbackBox.textContent = "";
    }

    function getCurrentStep() {
        return steps.find(x => x.stepIndex === state.currentStep);
    }

    function getCustomerFormData() {
        return {
            name: customerNameInput?.value?.trim() || "",
            email: customerEmailInput?.value?.trim() || "",
            phone: customerPhoneInput?.value?.trim() || "",
            taxOffice: customerTaxOfficeInput?.value?.trim() || "",
            taxNumber: customerTaxNumberInput?.value?.trim() || "",
            address: customerNotificationAddressInput?.value?.trim() || ""
        };
    }

    function displayValue(value, fallback = "................................................") {
        return value && value.trim() ? escapeHtml(value.trim()) : fallback;
    }

    function getTodayString() {
        return new Intl.DateTimeFormat("tr-TR", {
       day: "2-digit",
       month: "2-digit",
       year: "numeric"
   }).format(new Date());
    }

    function getContractDurationMonths() {
        const rawValue = Number(state.contractDurationMonths || 1);
        const safeValue = Math.max(parseInt(rawValue, 10) || 1, 1);
        return safeValue;
    }
    function getAvailablePaymentPlans(contractDurationMonths = getContractDurationMonths()) {
        return (wizardData.paymentPlans || [])
            .filter(plan => Number(plan.numberOfInstallments || 0) <= contractDurationMonths)
            .sort((a, b) => {
                const aCount = Number(a.numberOfInstallments || 0);
                const bCount = Number(b.numberOfInstallments || 0);
                return aCount - bCount;
            });
    }

    function syncPaymentPlanAvailability() {
        const contractDurationMonths = getContractDurationMonths();
        const availablePlans = getAvailablePaymentPlans(contractDurationMonths);
        const availablePlanIds = availablePlans.map(x => Number(x.id));

        document.querySelectorAll(".payment-tab").forEach(tab => {
            const planId = Number(tab.dataset.planId);
            const isAllowed = availablePlanIds.includes(planId);

            tab.disabled = !isAllowed;
            tab.classList.toggle("disabled", !isAllowed);

            if (!isAllowed) {
                tab.classList.remove("active");
            }
        });

        if (!availablePlanIds.length) {
            state.selectedPaymentPlanId = null;
            return;
        }

        const currentPlanStillValid = availablePlanIds.includes(Number(state.selectedPaymentPlanId));

        if (!currentPlanStillValid) {
            const bestMatchedPlan = availablePlans[availablePlans.length - 1];
            state.selectedPaymentPlanId = bestMatchedPlan ? Number(bestMatchedPlan.id) : Number(availablePlans[0].id);
        }

        document.querySelectorAll(".payment-tab").forEach(tab => {
            const planId = Number(tab.dataset.planId);
            tab.classList.toggle("active", planId === Number(state.selectedPaymentPlanId));
        });
    }
    function getSelectedItems() {
        const items = [];

        Object.keys(state.selectedPackages).forEach(categoryId => {
       state.selectedPackages[categoryId].forEach(packageId => {
           const item = packageLookup[packageId];
           if (!item) return;

           const quantity = item.isPiece
               ? Math.max(parseInt(state.selectedPackagePieces[packageId] || 1, 10), 1)
               : 1;

           const amount = item.isPiece
               ? round2(item.price * quantity)
               : round2(item.price);

           items.push({
               key: `package_${item.id}`,
               itemType: "package",
               id: item.id,
               categoryId: item.categoryId,
               categoryName: item.categoryName,
               name: item.isPiece ? `${item.name} x ${quantity}` : item.name,
               rawName: item.name,
               unitPrice: round2(item.price),
               amount: amount,
               quantity: quantity,
               isOneTime: item.isOneTime,
               isPiece: item.isPiece
           });
       });
   });

        Object.keys(state.selectedSubPackages).forEach(categoryId => {
       state.selectedSubPackages[categoryId].forEach(subPackageId => {
           const item = subPackageLookup[subPackageId];
           if (!item) return;

           const quantity = item.isPiece
               ? Math.max(parseInt(state.subPackagePieces[subPackageId] || 1, 10), 1)
               : 1;

           const amount = item.isPiece
               ? round2(item.price * quantity)
               : round2(item.price);

           items.push({
               key: `subpackage_${item.id}`,
               itemType: "subpackage",
               id: item.id,
               categoryId: item.categoryId,
               categoryName: item.categoryName,
               name: item.isPiece ? `${item.name} x ${quantity}` : item.name,
               rawName: item.name,
               unitPrice: round2(item.price),
               amount: amount,
               quantity: quantity,
               isOneTime: item.isOneTime,
               isPiece: item.isPiece
           });
       });
   });

        const total = round2(items.reduce((sum, item) => sum + item.amount, 0));
        return { items, total };
    }

    function splitAmountEvenly(total, count) {
        if (count <= 0) return [];
        if (total <= 0) return Array.from({ length: count }, () => 0);

        const base = Math.floor((total / count) * 100) / 100;
        const result = Array.from({ length: count }, () => base);

        let distributed = round2(base * count);
        let remainder = round2(total - distributed);

        let index = 0;
        while (remainder > 0 && index < count) {
            result[index] = round2(result[index] + 0.01);
            remainder = round2(remainder - 0.01);
            index++;
        }

        return result.map(x => round2(x));
    }

function buildPaymentPlanSummary(planId, requestedDiscountRate) {
    const contractDurationMonths = getContractDurationMonths();
    const availablePlans = getAvailablePaymentPlans(contractDurationMonths);
    const availablePlanIds = availablePlans.map(x => Number(x.id));

    let resolvedPlanId = Number(planId || 0);

    if (!availablePlanIds.includes(resolvedPlanId)) {
        const bestMatchedPlan = availablePlans[availablePlans.length - 1];
        resolvedPlanId = bestMatchedPlan ? Number(bestMatchedPlan.id) : 0;
    }

    const plan = wizardData.paymentPlans.find(x => Number(x.id) === resolvedPlanId);

    if (!plan) {
        return {
            plan: null,
            grossTotal: 0,
            discountAmount: 0,
            netTotal: 0,
            appliedDiscountRate: 0,
            requestedDiscountRate: 0,
            warning: "Bu sözleşme süresine uygun ödeme planı bulunamadı.",
            installments: [],
            contractDurationMonths
        };
    }

    const { items } = getSelectedItems();
    const installmentCount = Math.max(parseInt(plan.numberOfInstallments || 1, 10), 1);

    const installments = Array.from({ length: installmentCount }, (_, index) => ({
       month: index + 1,
       gross: 0,
       net: 0,
       lines: []
   }));

    const recurringItems = items.filter(x => !x.isOneTime);
    const oneTimeItems = items.filter(x => x.isOneTime);

    recurringItems.forEach(item => {
       const contractAmount = round2(item.amount * contractDurationMonths);
       const shares = splitAmountEvenly(contractAmount, installmentCount);

       shares.forEach((share, index) => {
           if (share <= 0) return;

           installments[index].gross = round2(installments[index].gross + share);
           installments[index].lines.push({
               name: `${item.name} (${contractDurationMonths} ay sözleşme)`,
               amount: share,
               type: "recurring"
           });
       });
   });

    oneTimeItems.forEach(item => {
       installments[0].gross = round2(installments[0].gross + item.amount);
       installments[0].lines.push({
           name: `${item.name} (tek seferlik)`,
           amount: item.amount,
           type: "oneTime"
       });
   });

    const maxDiscountRate = Number(plan.discountRate || 0);
    const safeRequestedDiscountRate = Math.min(Math.max(Number(requestedDiscountRate || 0), 0), 100);
    const appliedDiscountRate = Math.min(safeRequestedDiscountRate, maxDiscountRate);

    let warning = "";
    if (safeRequestedDiscountRate > maxDiscountRate) {
        warning = `Bu ödeme planında %${maxDiscountRate}'dan fazla indirim uygulanamaz.`;
    }

    const grossTotal = round2(installments.reduce((sum, installment) => sum + installment.gross, 0));
    const discountAmount = round2(grossTotal * appliedDiscountRate / 100);
    const netTotal = round2(grossTotal - discountAmount);

    let remainingNet = netTotal;

    installments.forEach((installment, index) => {
       if (index < installments.length - 1) {
           installment.net = round2(installment.gross * (1 - (appliedDiscountRate / 100)));
           remainingNet = round2(remainingNet - installment.net);
       } else {
           installment.net = round2(remainingNet);
       }
   });

    return {
        plan,
        contractDurationMonths,
        grossTotal,
        discountAmount,
        netTotal,
        appliedDiscountRate,
        requestedDiscountRate: safeRequestedDiscountRate,
        warning,
        installments
    };
}

    function getDisplayItemName(item, contractDurationMonths = getContractDurationMonths()) {
        const baseName = item?.rawName || item?.name || "";

        if (!item?.isOneTime && contractDurationMonths > 1) {
            return `${baseName} (${contractDurationMonths} ay)`;
        }

        return baseName;
    }

    function getDisplayItemAmount(item, contractDurationMonths = getContractDurationMonths()) {
        const baseAmount = round2(item?.amount || 0);
        return item?.isOneTime ? baseAmount : round2(baseAmount * contractDurationMonths);
    }

    function updateHiddenInputs(summary) {
        if (selectedPackagesJsonInput) {
            selectedPackagesJsonInput.value = JSON.stringify(state.selectedPackages);
        }

        if (selectedPackagePiecesJsonInput) {
            selectedPackagePiecesJsonInput.value = JSON.stringify(state.selectedPackagePieces);
        }

        if (selectedSubPackagesJsonInput) {
            selectedSubPackagesJsonInput.value = JSON.stringify(state.selectedSubPackages);
        }

        if (selectedSubPackagePiecesJsonInput) {
            selectedSubPackagePiecesJsonInput.value = JSON.stringify(state.subPackagePieces);
        }

        if (selectedPaymentPlanIdInput) {
            selectedPaymentPlanIdInput.value = state.selectedPaymentPlanId || "";
        }

        if (contractDurationMonthsInput) {
            contractDurationMonthsInput.value = summary?.contractDurationMonths ?? getContractDurationMonths();
        }

        if (appliedDiscountRateInput) {
            appliedDiscountRateInput.value = summary?.appliedDiscountRate ?? 0;
        }

        if (selectedDocumentTypeInput) {
            selectedDocumentTypeInput.value = state.selectedDocumentType || "proposal";
        }
    }

    function buildItemsTableRows(items, startIndex = 0, contractDurationMonths = getContractDurationMonths()) {
        if (!items.length) {
            return `
                <tr>
                    <td colspan="4">Herhangi bir hizmet seçilmedi.</td>
                </tr>
            `;
        }

        return items.map((item, index) => `
            <tr>
                <td>${startIndex + index + 1}</td>
                <td>${escapeHtml(getDisplayItemName(item, contractDurationMonths))}</td>
                <td>${item.quantity || 1}</td>
                <td>${formatCurrency(getDisplayItemAmount(item, contractDurationMonths))}</td>
            </tr>
        `).join("");
    }

    function buildInstallmentRows(installments) {
        if (!installments.length) {
            return `
                <tr>
                    <td colspan="2">Taksit bilgisi bulunmuyor.</td>
                </tr>
            `;
        }

        return installments.map(x => `
            <tr>
                <td>${x.month}. Ay</td>
                <td>${formatCurrency(x.net)}</td>
            </tr>
        `).join("");
    }

    function buildPdfPageHeader(docLabel) {
        return `
            <div class="pdf-page-header">
                <div class="pdf-page-brand">
                    <div class="pdf-logo-box">
                        <img src="${escapeAttr(offerPdfLogoUrl)}"
                             alt="Ideio Creative Logo"
                             onerror="this.style.display='none'; this.nextElementSibling.style.display='flex';" />
                        <div class="pdf-logo-fallback">ID</div>
                    </div>
                    <div>
                        <div class="pdf-brand-title">Ideio Creative</div>
                        <div class="pdf-brand-subtitle">${escapeHtml(docLabel)}</div>
                    </div>
                </div>
                <div class="pdf-page-meta">
                    <div><strong>Tarih:</strong> ${getTodayString()}</div>
                </div>
            </div>
        `;
    }


    function buildPdfPage(bodyHtml, pageNo, totalPages, docLabel) {
        return `
            <section class="document-preview-page">
                ${buildPdfPageHeader(docLabel)}
                <div class="pdf-page-content">
                    ${bodyHtml}
                </div>
                <div class="pdf-page-footer">
                    <span>${escapeHtml(docLabel)}</span>
                    <span>Sayfa ${pageNo} / ${totalPages}</span>
                </div>
            </section>
        `;
    }

    function createPdfPageElement(docLabel) {
        const pageEl = document.createElement("section");
        pageEl.className = "document-preview-page";
        pageEl.innerHTML = `
            ${buildPdfPageHeader(docLabel)}
            <div class="pdf-page-content"></div>
            <div class="pdf-page-footer">
                <span>${escapeHtml(docLabel)}</span>
                <span class="page-number-text"></span>
            </div>
        `;
        return pageEl;
    }

    function getOrCreatePaginationSandbox() {
        let sandbox = document.getElementById("documentPaginationSandbox");

        if (!sandbox) {
            sandbox = document.createElement("div");
            sandbox.id = "documentPaginationSandbox";
            sandbox.className = "document-pagination-sandbox";
            sandbox.setAttribute("aria-hidden", "true");
            document.body.appendChild(sandbox);
        }

        sandbox.innerHTML = "";
        return sandbox;
    }

    function isPageOverflowing(pageEl) {
        const contentEl = pageEl?.querySelector(".pdf-page-content");
        if (!contentEl) return false;

        return (contentEl.scrollHeight - contentEl.clientHeight) > 1;
    }

    function buildSectionContinuationTitle(titleEl) {
        if (!titleEl) return null;

        const clonedTitle = titleEl.cloneNode(true);
        const plainText = (clonedTitle.textContent || "").trim();

        if (plainText && !plainText.toLowerCase().includes("devam")) {
            clonedTitle.textContent = `${plainText} (Devam)`;
        }

        return clonedTitle;
    }

    function cloneDirectChildren(elements) {
        return (elements || []).map(x => x.cloneNode(true));
    }

    function getDirectChildByClass(parent, className) {
        return Array.from(parent?.children || []).find(x => x.classList?.contains(className)) || null;
    }

    function buildDocumentBlocks(rawPages) {
        const root = document.createElement("div");
        root.innerHTML = (rawPages || []).join("");

        const blocks = [];
        const nodes = Array.from(root.childNodes || []);

        for (let i = 0; i < nodes.length; i++) {
            const current = nodes[i];

            if (current.nodeType === Node.TEXT_NODE) {
                if ((current.textContent || "").trim()) {
                    const textBlock = document.createElement("div");
                    textBlock.className = "doc-free-text-block";
                    textBlock.textContent = current.textContent.trim();
                    blocks.push(textBlock);
                }
                continue;
            }

            if (current.nodeType !== Node.ELEMENT_NODE) {
                continue;
            }

            if (current.classList.contains("doc-title")) {
                const wrapper = document.createElement("div");
                wrapper.className = "doc-flow-heading";
                wrapper.appendChild(current.cloneNode(true));

                const next = nodes[i + 1];
                if (next && next.nodeType === Node.ELEMENT_NODE && next.classList.contains("doc-subtitle")) {
                    wrapper.appendChild(next.cloneNode(true));
                    i += 1;
                }

                blocks.push(wrapper);
                continue;
            }

            if (current.classList.contains("doc-subtitle")) {
                const wrapper = document.createElement("div");
                wrapper.className = "doc-flow-heading";
                wrapper.appendChild(current.cloneNode(true));
                blocks.push(wrapper);
                continue;
            }

            blocks.push(current.cloneNode(true));
        }

        return blocks;
    }

    function measureBlockFitsOnFreshPage(blockEl, sandbox, docLabel) {
        sandbox.innerHTML = "";

        const pageEl = createPdfPageElement(docLabel);
        sandbox.appendChild(pageEl);
        pageEl.querySelector(".pdf-page-content").appendChild(blockEl.cloneNode(true));

        return !isPageOverflowing(pageEl);
    }

    function splitSectionByListItems(sectionEl, sandbox, docLabel) {
        const listEl = getDirectChildByClass(sectionEl, "doc-list");
        if (!listEl) return [sectionEl.cloneNode(true)];

        const titleEl = getDirectChildByClass(sectionEl, "doc-section-title");
        const directChildren = Array.from(sectionEl.children || []);
        const listIndex = directChildren.indexOf(listEl);
        const beforeList = directChildren.slice(0, listIndex).filter(x => x !== titleEl);
        const afterList = directChildren.slice(listIndex + 1);
        const items = Array.from(listEl.children || []).filter(x => x.tagName === "LI");

        if (!items.length) return [sectionEl.cloneNode(true)];

        const groups = [];
        let currentGroup = [];

        for (const item of items) {
            const trialGroup = currentGroup.concat(item);
            const trialSection = document.createElement("div");
            trialSection.className = sectionEl.className;

            const heading = currentGroup.length ? buildSectionContinuationTitle(titleEl) : titleEl?.cloneNode(true);
            if (heading) trialSection.appendChild(heading);

            cloneDirectChildren(beforeList).forEach(x => trialSection.appendChild(x));

            const listClone = listEl.cloneNode(false);
            if (listEl.tagName === "OL") {
                const startValue = Number(listEl.getAttribute("start") || 1);
                const previousCount = groups.reduce((sum, group) => sum + group.length, 0);
                listClone.setAttribute("start", String(startValue + previousCount));
            }

            trialGroup.forEach(li => listClone.appendChild(li.cloneNode(true)));
            trialSection.appendChild(listClone);

            if (measureBlockFitsOnFreshPage(trialSection, sandbox, docLabel)) {
                currentGroup = trialGroup;
                continue;
            }

            if (!currentGroup.length) {
                currentGroup = [item];
                continue;
            }

            groups.push(currentGroup);
            currentGroup = [item];
        }

        if (currentGroup.length) {
            groups.push(currentGroup);
        }

        const result = [];
        let consumedItems = 0;

        groups.forEach((group, index) => {
            const chunk = document.createElement("div");
            chunk.className = sectionEl.className;

            const heading = index > 0 ? buildSectionContinuationTitle(titleEl) : titleEl?.cloneNode(true);
            if (heading) chunk.appendChild(heading);

            cloneDirectChildren(beforeList).forEach(x => chunk.appendChild(x));

            const listClone = listEl.cloneNode(false);
            if (listEl.tagName === "OL") {
                const startValue = Number(listEl.getAttribute("start") || 1);
                listClone.setAttribute("start", String(startValue + consumedItems));
            }

            group.forEach(li => listClone.appendChild(li.cloneNode(true)));
            chunk.appendChild(listClone);
            consumedItems += group.length;

            if (index === groups.length - 1) {
                cloneDirectChildren(afterList).forEach(x => chunk.appendChild(x));
            }

            result.push(chunk);
        });

        return result.length ? result : [sectionEl.cloneNode(true)];
    }

    function buildSectionTableChunk(sectionEl, titleEl, beforeTable, tableWrapEl, tableEl, theadEl, colgroupEl, rows, afterTable, chunkIndex, isLastChunk) {
        const chunk = document.createElement("div");
        chunk.className = sectionEl.className;

        const heading = chunkIndex > 0 ? buildSectionContinuationTitle(titleEl) : titleEl?.cloneNode(true);
        if (heading) chunk.appendChild(heading);

        cloneDirectChildren(beforeTable).forEach(x => chunk.appendChild(x));

        const tableWrapClone = tableWrapEl.cloneNode(false);
        const tableClone = tableEl.cloneNode(false);

        if (colgroupEl) {
            tableClone.appendChild(colgroupEl.cloneNode(true));
        }

        if (theadEl) {
            tableClone.appendChild(theadEl.cloneNode(true));
        }

        const tbodyClone = document.createElement("tbody");
        rows.forEach(row => tbodyClone.appendChild(row.cloneNode(true)));
        tableClone.appendChild(tbodyClone);
        tableWrapClone.appendChild(tableClone);
        chunk.appendChild(tableWrapClone);

        if (isLastChunk) {
            cloneDirectChildren(afterTable).forEach(x => chunk.appendChild(x));
        }

        return chunk;
    }

    function splitSectionByTableRows(sectionEl, sandbox, docLabel) {
        const tableWrapEl = getDirectChildByClass(sectionEl, "doc-table-wrap");
        if (!tableWrapEl) return [sectionEl.cloneNode(true)];

        const tableEl = tableWrapEl.querySelector(".doc-table, table");
        const tbodyEl = tableEl?.querySelector("tbody");
        const rows = Array.from(tbodyEl?.children || []).filter(x => x.tagName === "TR");
        const titleEl = getDirectChildByClass(sectionEl, "doc-section-title");
        const directChildren = Array.from(sectionEl.children || []);
        const tableIndex = directChildren.indexOf(tableWrapEl);
        const beforeTable = directChildren.slice(0, tableIndex).filter(x => x !== titleEl);
        const afterTable = directChildren.slice(tableIndex + 1);
        const theadEl = tableEl?.querySelector("thead");
        const colgroupEl = tableEl?.querySelector("colgroup");

        if (!tableEl || !tbodyEl || !rows.length) {
            return [sectionEl.cloneNode(true)];
        }

        const groups = [];
        let currentGroup = [];

        for (const row of rows) {
            const trialGroup = currentGroup.concat(row);
            const trialChunk = buildSectionTableChunk(sectionEl, titleEl, beforeTable, tableWrapEl, tableEl, theadEl, colgroupEl, trialGroup, [], groups.length, false);

            if (measureBlockFitsOnFreshPage(trialChunk, sandbox, docLabel)) {
                currentGroup = trialGroup;
                continue;
            }

            if (!currentGroup.length) {
                currentGroup = [row];
                continue;
            }

            groups.push(currentGroup);
            currentGroup = [row];
        }

        if (currentGroup.length) {
            groups.push(currentGroup);
        }

        const result = groups.map((group, index) =>
            buildSectionTableChunk(sectionEl, titleEl, beforeTable, tableWrapEl, tableEl, theadEl, colgroupEl, group, afterTable, index, index === groups.length - 1)
        );

        return result.length ? result : [sectionEl.cloneNode(true)];
    }

    function buildSectionInfoGridChunk(sectionEl, titleEl, beforeGrid, gridEl, items, afterGrid, chunkIndex, isLastChunk) {
        const chunk = document.createElement("div");
        chunk.className = sectionEl.className;

        const heading = chunkIndex > 0 ? buildSectionContinuationTitle(titleEl) : titleEl?.cloneNode(true);
        if (heading) chunk.appendChild(heading);

        cloneDirectChildren(beforeGrid).forEach(x => chunk.appendChild(x));

        const gridClone = gridEl.cloneNode(false);
        items.forEach(item => gridClone.appendChild(item.cloneNode(true)));
        chunk.appendChild(gridClone);

        if (isLastChunk) {
            cloneDirectChildren(afterGrid).forEach(x => chunk.appendChild(x));
        }

        return chunk;
    }

    function splitSectionByInfoItems(sectionEl, sandbox, docLabel) {
        const gridEl = getDirectChildByClass(sectionEl, "doc-info-grid");
        if (!gridEl) return [sectionEl.cloneNode(true)];

        const items = Array.from(gridEl.children || []).filter(x => x.classList?.contains("doc-info-item"));
        if (!items.length) return [sectionEl.cloneNode(true)];

        const titleEl = getDirectChildByClass(sectionEl, "doc-section-title");
        const directChildren = Array.from(sectionEl.children || []);
        const gridIndex = directChildren.indexOf(gridEl);
        const beforeGrid = directChildren.slice(0, gridIndex).filter(x => x !== titleEl);
        const afterGrid = directChildren.slice(gridIndex + 1);

        const groups = [];
        let currentGroup = [];

        for (const item of items) {
            const trialGroup = currentGroup.concat(item);
            const trialChunk = buildSectionInfoGridChunk(sectionEl, titleEl, beforeGrid, gridEl, trialGroup, [], groups.length, false);

            if (measureBlockFitsOnFreshPage(trialChunk, sandbox, docLabel)) {
                currentGroup = trialGroup;
                continue;
            }

            if (!currentGroup.length) {
                currentGroup = [item];
                continue;
            }

            groups.push(currentGroup);
            currentGroup = [item];
        }

        if (currentGroup.length) {
            groups.push(currentGroup);
        }

        const result = groups.map((group, index) =>
            buildSectionInfoGridChunk(sectionEl, titleEl, beforeGrid, gridEl, group, afterGrid, index, index === groups.length - 1)
        );

        return result.length ? result : [sectionEl.cloneNode(true)];
    }

    function splitOversizedBlock(blockEl, sandbox, docLabel) {
        if (!blockEl || blockEl.nodeType !== Node.ELEMENT_NODE) {
            return [blockEl.cloneNode(true)];
        }

        if (!blockEl.classList) {
            return [blockEl.cloneNode(true)];
        }

        if (!blockEl.classList.contains("doc-section")) {
            return [blockEl.cloneNode(true)];
        }

        if (getDirectChildByClass(blockEl, "doc-table-wrap")) {
            return splitSectionByTableRows(blockEl, sandbox, docLabel);
        }

        if (getDirectChildByClass(blockEl, "doc-list")) {
            return splitSectionByListItems(blockEl, sandbox, docLabel);
        }

        if (getDirectChildByClass(blockEl, "doc-info-grid")) {
            return splitSectionByInfoItems(blockEl, sandbox, docLabel);
        }

        return [blockEl.cloneNode(true)];
    }

    function buildAutoPaginatedPages(rawPages, docLabel) {
        const blocks = buildDocumentBlocks(rawPages);
        const sandbox = getOrCreatePaginationSandbox();
        const pages = [];

        let currentPage = createPdfPageElement(docLabel);
        sandbox.appendChild(currentPage);
        pages.push(currentPage);

        const createNewPage = () => {
            currentPage = createPdfPageElement(docLabel);
            sandbox.appendChild(currentPage);
            pages.push(currentPage);
        };

        const queue = blocks.map(x => x.cloneNode(true));
        let guard = 0;

        while (queue.length && guard < 10000) {
            guard += 1;

            const block = queue.shift();
            const contentEl = currentPage.querySelector(".pdf-page-content");
            const trialClone = block.cloneNode(true);
            contentEl.appendChild(trialClone);

            if (!isPageOverflowing(currentPage)) {
                continue;
            }

            contentEl.removeChild(trialClone);

            if (contentEl.children.length > 0) {
                createNewPage();
                const freshContentEl = currentPage.querySelector(".pdf-page-content");
                const freshClone = block.cloneNode(true);
                freshContentEl.appendChild(freshClone);

                if (!isPageOverflowing(currentPage)) {
                    continue;
                }

                freshContentEl.removeChild(freshClone);
            }

            const splitBlocks = splitOversizedBlock(block, sandbox, docLabel);
            const isUnchanged = splitBlocks.length === 1 && splitBlocks[0].outerHTML === block.outerHTML;

            if (isUnchanged) {
                currentPage.querySelector(".pdf-page-content").appendChild(block.cloneNode(true));
                continue;
            }

            for (let i = splitBlocks.length - 1; i >= 0; i -= 1) {
                queue.unshift(splitBlocks[i].cloneNode(true));
            }
        }

        if (!pages.length) {
            return [];
        }

        const totalPages = pages.length;
        pages.forEach((pageEl, index) => {
            const numberEl = pageEl.querySelector(".page-number-text");
            if (numberEl) {
                numberEl.textContent = `Sayfa ${index + 1} / ${totalPages}`;
            }
        });

        const finalPages = pages.map(pageEl => pageEl.outerHTML);
        sandbox.innerHTML = "";
        return finalPages;
    }

    function buildProposalPages(customer, items, summary) {
        const pages = [];
        const itemChunks = chunkArray(items, 9999);
        const installmentChunks = chunkArray(summary.installments || [], 9999);

        const firstItemChunk = itemChunks[0] || [];
        const firstInstallmentChunk = installmentChunks[0] || [];

        const serviceProvider = {
            companyName: "Ideio Creative Tasarım Reklamcılık Sosyal Medya Hizmetleri Sanayi ve Ticaret Limited Şirketi",
            address: "Mansuroğlu, 287. Sk. No:6 Daire:2, 35530 Bayraklı/İzmir",
            taxOffice: "Bornova Vergi Dairesi",
            taxNumber: "4651560067",
            email: "info@ideiocreative.com",
            iban: "TR11 0006 2001 2750 0006 2961 41",
            bankName: "Garanti Bankası",
            accountHolder: "IDEIOCREATIVE Tasarım Reklamcılık Sosyal Medya Hiz. San. ve Tic. Ltd. Şti."
        };

        function safe(val, fallback = "-") {
            if (val === null || val === undefined) return fallback;
            const str = String(val).trim();
            return str ? escapeHtml(str) : fallback;
        }

        function formatDate(val, fallback = "..... / ..... / ..........") {
            if (!val) return fallback;

            const d = new Date(val);
            if (!isNaN(d.getTime())) {
                const day = String(d.getDate()).padStart(2, "0");
                const month = String(d.getMonth() + 1).padStart(2, "0");
                const year = d.getFullYear();
                return `${day}.${month}.${year}`;
            }

            return escapeHtml(String(val));
        }

        function getProtocolStartDate() {
            return summary.protocolDate || summary.contractStartDate || summary.startDate || summary.firstPaymentDate || "";
        }

        function getProtocolEndDate() {
            if (summary.protocolEndDate) return summary.protocolEndDate;
            if (summary.contractEndDate) return summary.contractEndDate;

            const start = getProtocolStartDate();
            const months = Number(summary.contractDurationMonths || 0);

            if (start && months > 0) {
                const d = new Date(start);
                if (!isNaN(d.getTime())) {
                    d.setMonth(d.getMonth() + months);
                    return d;
                }
            }

            return "";
        }

        function getSelectedPackageName() {
            return summary.selectedPackageName || summary.plan?.name || "Seçilen Paket";
        }

        function getMonthlyFeeText() {
            if (summary.monthlyFee !== undefined && summary.monthlyFee !== null) {
                return `${formatCurrency(summary.monthlyFee)} + KDV`;
            }

            if ((summary.contractDurationMonths || 0) > 0 && (summary.netTotal || 0) > 0) {
                const monthly = Number(summary.netTotal) / Number(summary.contractDurationMonths);
                return `${formatCurrency(monthly)} + KDV`;
            }

            return `${formatCurrency(summary.netTotal || 0)} + KDV`;
        }

        function getPaymentPlanText() {
            return summary.plan ? escapeHtml(summary.plan.name) : "-";
        }

        function getSocialAccountsText() {
            const accounts = [];

            if (customer.instagram) accounts.push(`${escapeHtml(customer.instagram)} Instagram`);
            if (customer.youtube) accounts.push(`${escapeHtml(customer.youtube)} YouTube`);
            if (customer.tiktok) accounts.push(`${escapeHtml(customer.tiktok)} TikTok`);
            if (customer.facebook) accounts.push(`${escapeHtml(customer.facebook)} Facebook`);
            if (customer.linkedin) accounts.push(`${escapeHtml(customer.linkedin)} LinkedIn`);

            if (!accounts.length) return "Hizmet Alan’a ait sosyal medya hesapları";

            if (accounts.length === 1) return accounts[0];
            if (accounts.length === 2) return `${accounts[0]} ve ${accounts[1]}`;

            return `${accounts.slice(0, -1).join(", ")} ve ${accounts[accounts.length - 1]}`;
        }

        function getAdContributionText() {
            if (summary.adBudgetContributionRate !== undefined && summary.adBudgetContributionRate !== null) {
                return `%${escapeHtml(String(summary.adBudgetContributionRate))}`;
            }

            const selected = getSelectedPackageName().toLowerCase();
            if (selected.includes("temel")) return "%20";
            if (selected.includes("pro")) return "%0";
            if (selected.includes("premium")) return "%0";

            return "%0";
        }

        pages.push(`
        <div class="doc-title">Hizmet Ön Protokolü</div>
        <div class="doc-subtitle">
            Sayın ${displayValue(customer.name, "Müşteri")} için hazırlanan hizmet kapsamı ve ticari teklif ön protokolüdür.
        </div>

        <div class="doc-section">
            <div class="doc-section-title">1. Taraf Bilgileri</div>
            <div class="doc-info-grid">
                <div class="doc-info-item full">
                    <span>Hizmet Veren</span>
                    <strong>${safe(serviceProvider.companyName)}</strong>
                </div>
                <div class="doc-info-item full">
                    <span>Adres</span>
                    <strong>${safe(serviceProvider.address)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi Dairesi</span>
                    <strong>${safe(serviceProvider.taxOffice)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi No</span>
                    <strong>${safe(serviceProvider.taxNumber)}</strong>
                </div>
                <div class="doc-info-item full">
                    <span>E-Posta</span>
                    <strong>${safe(serviceProvider.email)}</strong>
                </div>
            </div>

            <div class="doc-info-grid" style="margin-top:16px;">
                <div class="doc-info-item full">
                    <span>Hizmet Alan / Firma</span>
                    <strong>${displayValue(customer.name, "Müşteri / Firma")}</strong>
                </div>
                <div class="doc-info-item">
                    <span>E-Posta</span>
                    <strong>${displayValue(customer.email)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Telefon</span>
                    <strong>${displayValue(customer.phone)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi Dairesi</span>
                    <strong>${displayValue(customer.taxOffice)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi No</span>
                    <strong>${displayValue(customer.taxNumber)}</strong>
                </div>
                <div class="doc-info-item full">
                    <span>Tebligat Adresi</span>
                    <strong>${displayValue(customer.address)}</strong>
                </div>
            </div>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">2. Ön Protokolün Konusu</div>
            <div class="doc-note">
                İşbu ön protokolün konusu; Hizmet Veren tarafından Hizmet Alan’a sunulacak reklam, tanıtım, dijital medya yönetimi,
                içerik üretimi, grafik ve görsel tasarım, medya planlama, sosyal medya yönetimi ve ilgili hizmetlerin kapsamı ile
                ticari koşulların ön mutabakat altına alınmasıdır.
            </div>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">3. Ön Protokol Süresi</div>
            <div class="doc-table-wrap">
                <table class="doc-table">
                    <tbody>
                        <tr>
                            <th>Başlangıç Tarihi</th>
                            <td>${formatDate(getProtocolStartDate())}</td>
                        </tr>
                        <tr>
                            <th>Bitiş Tarihi</th>
                            <td>${formatDate(getProtocolEndDate())}</td>
                        </tr>
                        <tr>
                            <th>Hizmet Süresi</th>
                            <td>${safe(summary.contractDurationMonths, "0")} Ay</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">4. Hizmet Kapsamı</div>
            <ol class="doc-list">
                <li>Hizmet Veren, Hizmet Alan’a ait ${getSocialAccountsText()} için içerik planlama, paylaşım ve yönetim hizmeti sunacaktır.</li>
                <li>Hizmet kapsamı seçilen paket ve hizmet kalemleri ile sınırlıdır.</li>
                <li>Tarafların yazılı mutabakatı ile kapsam genişletilebilir veya daraltılabilir.</li>
                <li>Sözleşme kapsamında ayrıca kararlaştırılmayan ek hizmetler ayrıca fiyatlandırılır.</li>
            </ol>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">5. Seçilen Paket ve Hizmet Kalemleri</div>
            <div class="doc-table-wrap" style="margin-bottom:14px;">
                <table class="doc-table">
                    <tbody>
                        <tr>
                            <th>Seçilen Paket</th>
                            <td>${safe(getSelectedPackageName())}</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="doc-table-wrap">
                <table class="doc-table">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Hizmet</th>
                            <th>Adet</th>
                            <th>Tutar</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${buildItemsTableRows(firstItemChunk, 0, summary.contractDurationMonths)}
                    </tbody>
                </table>
            </div>
        </div>
    `);

        if (itemChunks.length > 1) {
            for (let i = 1; i < itemChunks.length; i++) {
                pages.push(`
                <div class="doc-title">Seçilen Hizmet Kalemleri</div>
                <div class="doc-subtitle">Devam eden hizmet listesi</div>

                <div class="doc-section">
                    <div class="doc-table-wrap">
                        <table class="doc-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Hizmet</th>
                                    <th>Adet</th>
                                    <th>Tutar</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${buildItemsTableRows(itemChunks[i], i * 12, summary.contractDurationMonths)}
                            </tbody>
                        </table>
                    </div>
                </div>
            `);
            }
        }

        pages.push(`
        <div class="doc-title">Ticari Şartlar ve Ödeme Özeti</div>
        <div class="doc-subtitle">Ücretlendirme, ödeme planı ve genel ticari koşullar</div>

        <div class="doc-section">
            <div class="doc-section-title">6. Ücretlendirme</div>
            <ol class="doc-list">
                <li>Aylık hizmet bedeli <strong>${getMonthlyFeeText()}</strong> olarak öngörülmüştür.</li>
                <li>Toplam teklif tutarı, seçilen hizmet kalemleri ve uygulanan indirim oranına göre aşağıda belirtilmiştir.</li>
                <li>Seçilen pakette reklam yönetimi bulunması halinde reklam bütçesi katkı oranı <strong>${getAdContributionText()}</strong> olarak uygulanacaktır.</li>
                <li>Nihai fiyatlandırma, resmi sözleşme ve/veya fatura aşamasında kesinleşecektir.</li>
            </ol>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">7. Ödeme Planı Bilgileri</div>
            <div class="doc-table-wrap">
                <table class="doc-table">
                    <tbody>
                        <tr>
                            <th>Ödeme Planı</th>
                            <td>${getPaymentPlanText()}</td>
                        </tr>
                        <tr>
                            <th>Sözleşme Süresi</th>
                            <td>${safe(summary.contractDurationMonths, "0")} Ay</td>
                        </tr>
                        <tr>
                            <th>İndirim Oranı</th>
                            <td>%${safe(summary.appliedDiscountRate, "0")}</td>
                        </tr>
                        <tr>
                            <th>Brüt Toplam</th>
                            <td>${formatCurrency(summary.grossTotal || 0)}</td>
                        </tr>
                        <tr>
                            <th>İndirim Tutarı</th>
                            <td>${formatCurrency(summary.discountAmount || 0)}</td>
                        </tr>
                        <tr>
                            <th>Net Toplam</th>
                            <td>${formatCurrency(summary.netTotal || 0)}</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="doc-total-box">
                <div class="doc-total-row">
                    <span>Brüt Toplam</span>
                    <strong>${formatCurrency(summary.grossTotal || 0)}</strong>
                </div>
                <div class="doc-total-row">
                    <span>İndirim</span>
                    <strong>${formatCurrency(summary.discountAmount || 0)}</strong>
                </div>
                <div class="doc-total-row net">
                    <span>Net Toplam</span>
                    <strong>${formatCurrency(summary.netTotal || 0)}</strong>
                </div>
            </div>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">8. Taksit Dağılımı</div>
            <div class="doc-table-wrap">
                <table class="doc-table">
                    <thead>
                        <tr>
                            <th>Dönem</th>
                            <th>Tutar</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${buildInstallmentRows(firstInstallmentChunk)}
                    </tbody>
                </table>
            </div>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">9. Ödeme Hesap Bilgileri</div>
            <div class="doc-table-wrap">
                <table class="doc-table">
                    <tbody>
                        <tr>
                            <th>Banka Adı</th>
                            <td>${safe(serviceProvider.bankName)}</td>
                        </tr>
                        <tr>
                            <th>Hesap Sahibi</th>
                            <td>${safe(serviceProvider.accountHolder)}</td>
                        </tr>
                        <tr>
                            <th>IBAN</th>
                            <td>${safe(serviceProvider.iban)}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    `);

        if (installmentChunks.length > 1) {
            for (let i = 1; i < installmentChunks.length; i++) {
                pages.push(`
                <div class="doc-title">Taksit Dağılımı</div>
                <div class="doc-subtitle">Devam eden ödeme planı detayları</div>

                <div class="doc-section">
                    <div class="doc-table-wrap">
                        <table class="doc-table">
                            <thead>
                                <tr>
                                    <th>Dönem</th>
                                    <th>Tutar</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${buildInstallmentRows(installmentChunks[i])}
                            </tbody>
                        </table>
                    </div>
                </div>
            `);
            }
        }

        pages.push(`
        <div class="doc-title">Genel Şartlar ve Onay</div>
        <div class="doc-subtitle">Ön mutabakat, gizlilik ve onay bölümü</div>

        <div class="doc-section">
            <div class="doc-section-title">10. Genel Şartlar</div>
            <ol class="doc-list">
                <li>Bu belge ön protokol ve teklif niteliğindedir; nihai koşullar resmi sözleşme ve fatura ile kesinleşir.</li>
                <li>Hizmet kapsamı dışında talep edilen tüm ek işler ayrıca fiyatlandırılır.</li>
                <li>Taraflar arasında paylaşılacak ticari, stratejik ve operasyonel bilgiler gizli kabul edilir.</li>
                <li>Hizmet performansı; erişim, etkileşim, satış dönüşümü veya ticari kazanç bakımından garanti teşkil etmez.</li>
                <li>Resmi sözleşme imzalanması halinde bu ön protokol, sözleşme görüşmelerine esas ticari çerçeveyi oluşturur.</li>
            </ol>
        </div>

        <div class="doc-section">
            <div class="doc-note">
                İşbu ön protokol, tarafların teklif kapsamı ve ticari koşullara ilişkin ön mutabakatını göstermek amacıyla hazırlanmıştır.
            </div>
        </div>

        <div class="doc-signatures">
            <div class="doc-sign-box">
                <strong>Hizmet Veren</strong>
                <br><br>
                ${safe(serviceProvider.companyName)}
                <br>
                Yetkili: UĞUR FIRAT SOM
            </div>
            <div class="doc-sign-box">
                <strong>Hizmet Alan</strong>
                <br><br>
                ${displayValue(customer.name, "Müşteri / Firma")}
                <br>
                Yetkili: ${displayValue(customer.authorizedPerson || customer.name, "Yetkili")}
            </div>
        </div>
    `);

        return pages;
    }

  function buildContractPages(customer, items, summary) {
        const pages = [];

        // Daha güvenli satır adetleri:
        // Uzun hizmet isimleri / uzun taksit satırları taşma yapmasın diye düşürüldü.
        const ITEM_ROWS_PER_PAGE = 9999;
        const INSTALLMENT_ROWS_PER_PAGE = 9999;
        const LIST_ITEMS_PER_PAGE = 9999;

        const itemChunks = chunkArray(items || [], ITEM_ROWS_PER_PAGE);
        const installmentChunks = chunkArray(summary.installments || [], INSTALLMENT_ROWS_PER_PAGE);

        const serviceProvider = {
            companyName: "Ideio Creative Tasarım Reklamcılık Sosyal Medya Hizmetleri Sanayi ve Ticaret Limited Şirketi",
            address: "Mansuroğlu, 287. Sk. No:6 Daire:2, 35530 Bayraklı/İzmir",
            taxOffice: "Bornova Vergi Dairesi",
            taxNumber: "4651560067",
            email: "info@ideiocreative.com"
        };

        function safe(val, fallback = "-") {
            if (val === null || val === undefined) return fallback;
            const str = String(val).trim();
            return str ? escapeHtml(str) : fallback;
        }

        function formatDate(val, fallback = "..... / ..... / ..........") {
            if (!val) return fallback;

            const d = new Date(val);
            if (!isNaN(d.getTime())) {
                const day = String(d.getDate()).padStart(2, "0");
                const month = String(d.getMonth() + 1).padStart(2, "0");
                const year = d.getFullYear();
                return `${day}.${month}.${year}`;
            }

            return escapeHtml(String(val));
        }

        function getContractStartDate() {
            return summary.contractStartDate || summary.startDate || summary.firstPaymentDate || "";
        }

        function getContractEndDate() {
            if (summary.contractEndDate) return summary.contractEndDate;

            const start = getContractStartDate();
            const months = Number(summary.contractDurationMonths || 0);

            if (start && months > 0) {
                const d = new Date(start);
                if (!isNaN(d.getTime())) {
                    d.setMonth(d.getMonth() + months);
                    return d;
                }
            }

            return "";
        }

        function getSelectedPackageName() {
            if (summary.selectedPackageName) return summary.selectedPackageName;
            if (summary.plan && summary.plan.name) return summary.plan.name;
            return "Seçilen Paket";
        }

        function getPaymentTypeText() {
            if (summary.plan && summary.plan.name) return escapeHtml(summary.plan.name);
            return "Belirlenen ödeme planına göre";
        }

        function getPaymentDayText() {
            return safe(summary.paymentDay, "...");
        }

        function getFirstPaymentDateText() {
            return formatDate(summary.firstPaymentDate, "..... / ..... / ..........");
        }

        function getMonthlyFeeText() {
            if (summary.monthlyFee !== undefined && summary.monthlyFee !== null) {
                return `${formatCurrency(summary.monthlyFee)} + KDV`;
            }

            if ((summary.contractDurationMonths || 0) > 0 && (summary.netTotal || 0) > 0) {
                const monthly = Number(summary.netTotal) / Number(summary.contractDurationMonths);
                return `${formatCurrency(monthly)} + KDV`;
            }

            return `${formatCurrency(summary.netTotal || 0)} + KDV`;
        }

        function getSocialAccountsText() {
            const accounts = [];

            if (customer.instagram) accounts.push(`${escapeHtml(customer.instagram)} Instagram hesabı`);
            if (customer.youtube) accounts.push(`${escapeHtml(customer.youtube)} YouTube hesabı`);
            if (customer.tiktok) accounts.push(`${escapeHtml(customer.tiktok)} TikTok hesabı`);
            if (customer.facebook) accounts.push(`${escapeHtml(customer.facebook)} Facebook hesabı`);
            if (customer.linkedin) accounts.push(`${escapeHtml(customer.linkedin)} LinkedIn hesabı`);

            if (!accounts.length) {
                return "Hizmet Alan’a ait sosyal medya hesapları";
            }

            if (accounts.length === 1) return accounts[0];
            if (accounts.length === 2) return `${accounts[0]} ve ${accounts[1]}`;

            return `${accounts.slice(0, -1).join(", ")} ve ${accounts[accounts.length - 1]}`;
        }

        function getPackagePreferenceRows() {
            const selected = getSelectedPackageName().toLowerCase();

            const packageRows = [
                { name: "Ek–1: Sosyal Medya Paketi (Temel)", matchers: ["temel"] },
                { name: "Ek–2: Sosyal Medya Paketleri (Prodüksiyonsuz)", matchers: ["prodüksiyonsuz", "produksiyonsuz"] },
                { name: "Ek–3: İçerik Üretim Paketleri", matchers: ["içerik", "icerik"] },
                { name: "Ek–4: Dijital Pazarlama Paketi", matchers: ["dijital", "pazarlama"] }
            ];

            return packageRows.map((pkg) => {
                const isSelected =
                    pkg.matchers.some(x => selected.includes(x)) ||
                    selected.includes(pkg.name.toLowerCase());

                return `
                <tr>
                    <th>${pkg.name}</th>
                    <td>${isSelected ? "[Tercih Edilmiştir]" : "[Tercih Edilmemiştir]"}</td>
                </tr>
            `;
            }).join("");
        }

        function getAdContributionText() {
            if (summary.adBudgetContributionRate !== undefined && summary.adBudgetContributionRate !== null) {
                return `%${escapeHtml(String(summary.adBudgetContributionRate))}`;
            }

            const selected = getSelectedPackageName().toLowerCase();
            if (selected.includes("temel")) return "%20";
            if (selected.includes("pro")) return "%0";
            if (selected.includes("premium")) return "%0";

            return "%0";
        }

        function pushPage(title, subtitle, content) {
            pages.push(`
            <div class="doc-title">${title}</div>
            <div class="doc-subtitle">${subtitle}</div>
            ${content}
        `);
        }

        function buildClausePage(title, subtitle, content) {
            pushPage(title, subtitle, content);
        }

        function buildListPages(title, subtitle, sectionTitle, listItems, startNumber, perPage = LIST_ITEMS_PER_PAGE) {
            const chunks = chunkArray(listItems, perPage);

            chunks.forEach((chunk, pageIndex) => {
                const pageStart = startNumber + (pageIndex * perPage);

                pushPage(
                    title,
                    pageIndex === 0 ? subtitle : "Devamı",
                    `
                <div class="doc-section">
                    <div class="doc-section-title">
                        ${sectionTitle}${pageIndex > 0 ? " (Devam)" : ""}
                    </div>
                    <ol class="doc-list" start="${pageStart}">
                        ${chunk.map(item => `<li>${item}</li>`).join("")}
                    </ol>
                </div>
                `
                );
            });
        }

        // 1) Kapak + Taraflar
        buildClausePage(
            "Sosyal Medya Yönetimi ve Reklam Sözleşmesi",
            "İşbu sözleşme Hizmet Veren ile Hizmet Alan arasında düzenlenmiştir.",
            `
        <div class="doc-section">
            <div class="doc-section-title">1. Sözleşme Tarafları</div>

            <div class="doc-info-grid">
                <div class="doc-info-item full">
                    <span>Hizmet Veren</span>
                    <strong>${safe(serviceProvider.companyName)}</strong>
                </div>
                <div class="doc-info-item full">
                    <span>Adres</span>
                    <strong>${safe(serviceProvider.address)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi Dairesi</span>
                    <strong>${safe(serviceProvider.taxOffice)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi Numarası</span>
                    <strong>${safe(serviceProvider.taxNumber)}</strong>
                </div>
                <div class="doc-info-item full">
                    <span>E-Posta</span>
                    <strong>${safe(serviceProvider.email)}</strong>
                </div>
            </div>

            <div class="doc-info-grid" style="margin-top:16px;">
                <div class="doc-info-item full">
                    <span>Hizmet Alan</span>
                    <strong>${safe(customer.name, "Müşteri / Firma")}</strong>
                </div>
                <div class="doc-info-item full">
                    <span>Adres</span>
                    <strong>${safe(customer.address)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi Dairesi</span>
                    <strong>${safe(customer.taxOffice)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Vergi Numarası</span>
                    <strong>${safe(customer.taxNumber)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>E-Posta</span>
                    <strong>${safe(customer.email)}</strong>
                </div>
                <div class="doc-info-item">
                    <span>Telefon</span>
                    <strong>${safe(customer.phone)}</strong>
                </div>
            </div>
        </div>
        `
        );

        // 2) Konu + Süre
        buildClausePage(
            "Sözleşmenin Konusu ve Süresi",
            "Sözleşmenin dayanağı, konusu ve yürürlük süresi",
            `
        <div class="doc-section">
            <div class="doc-section-title">2. Sözleşmenin Konusu</div>
            <ol class="doc-list">
                <li>İşbu sözleşmenin konusu, Hizmet Veren tarafından Hizmet Alan’a reklam, tanıtım, dijital medya yönetimi, içerik üretimi, grafik ve görsel tasarım, medya planlama ve uygulama, sosyal medya yönetimi ile ilgili hizmetlerin sağlanmasına ilişkin usul ve esasların belirlenmesidir.</li>
            </ol>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">3. Sözleşme Süresi</div>
            <ol class="doc-list">
                <li>İşbu sözleşme <strong>${formatDate(getContractStartDate())}</strong> tarihinde yürürlüğe girer ve <strong>${formatDate(getContractEndDate())}</strong> tarihinde sona erer.</li>
                <li>Taraflardan herhangi biri, sözleşme süresinin sona ermesinden en az 30 gün önce yazılı olarak sözleşmenin yenilenmeyeceğini bildirmediği takdirde, sözleşme aynı bedel, şart ve koşullarla, mevcut sözleşme süresi kadar birbirini izleyen dönemler halinde kendiliğinden uzamış sayılır.</li>
            </ol>
        </div>
        `
        );

        // 3) Hizmet kapsamı
        buildClausePage(
            "Hizmetin Kapsamı",
            "Seçilen paketler ve sosyal medya hesapları",
            `
        <div class="doc-section">
            <div class="doc-section-title">4. Hizmetin Kapsamı</div>
            <ol class="doc-list">
                <li>Hizmet Veren, işbu sözleşme kapsamında Hizmet Alan’a ait olan ${getSocialAccountsText()} yönetimi ile içerik planlamasını, <strong>${safe(getSelectedPackageName())}</strong> çerçevesinde yürütmeyi kabul ve taahhüt eder.</li>
                <li>İşbu sözleşmeye ek olarak düzenlenen paket tercih durumu aşağıdaki gibidir:</li>
            </ol>

            <div class="doc-table-wrap">
                <table class="doc-table">
                    <tbody>
                        ${getPackagePreferenceRows()}
                    </tbody>
                </table>
            </div>

            <ol class="doc-list" start="3">
                <li>Hizmetin kapsamı tarafların yazılı mutabakatı ile genişletilebilir veya daraltılabilir.</li>
                <li>Tercih edilmeyen paketler, sözleşmenin eki olmakla birlikte taraflar açısından herhangi bir hak ve yükümlülük doğurmaz.</li>
                <li>Hizmetin kapsamına girmeyen işler bakımından Hizmet Veren herhangi bir sorumluluk altına girmez.</li>
            </ol>
        </div>
        `
        );

        // 4) Seçilen hizmetler tablosu
        itemChunks.forEach((chunk, index) => {
            pushPage(
                "Seçilen Hizmetler",
                index === 0 ? "Sözleşme kapsamına dahil hizmetler" : "Devam eden hizmet listesi",
                `
            <div class="doc-section">
                <div class="doc-table-wrap">
                    <table class="doc-table">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Hizmet</th>
                                <th>Adet</th>
                                <th>Tutar</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${buildItemsTableRows(chunk, index * ITEM_ROWS_PER_PAGE, summary.contractDurationMonths)}
                        </tbody>
                    </table>
                </div>
            </div>
            `
            );
        });

        // 5A) Ücretlendirme metni
        buildClausePage(
            "Ücretlendirme ve Ödeme Planı",
            "Sözleşmeye ait ücretlendirme bilgileri",
            `
        <div class="doc-section">
            <div class="doc-section-title">5. Ücretlendirme ve Ödeme</div>
            <ol class="doc-list">
                <li>Hizmet bedeli, aylık olarak <strong>${getMonthlyFeeText()}</strong> olarak belirlenmiştir.</li>
                <li>Hizmet Alan, işbu sözleşme kapsamında belirlenen hizmet bedelini, ilk ödeme <strong>${getFirstPaymentDateText()}</strong> tarihinde yapılmak üzere peşin olarak ödemeyi kabul ve taahhüt eder. Bu tarihten sonraki ödemeler ise her ayın <strong>${getPaymentDayText()}</strong>. günü vade tarihi olmak üzere Hizmet Veren’in banka hesabına yapılır.</li>
                <li>Tercih edilen paketin içeriğinde reklam hizmeti bulunması halinde, reklam bütçesi katkı payı oranı <strong>${getAdContributionText()}</strong> olarak uygulanır.</li>
                <li>Ödemelerin belirtilen tarihlerde yapılmaması halinde, Hizmet Alan temerrüde düşmüş sayılır ve Hizmet Veren, vade tarihinden itibaren aylık %3 gecikme faizi talep etme hakkına sahiptir.</li>
                <li>Ödeme gecikmesinin 30 günü aşması halinde Hizmet Veren işlerini durdurabilir; 45 günü aşması halinde sözleşmeyi haklı nedenle feshedebilir.</li>
            </ol>
        </div>
        `
        );

        // 5B) Ödeme özeti + banka bilgileri
        buildClausePage(
            "Ücretlendirme ve Ödeme Planı",
            "Ödeme özeti ve banka bilgileri",
            `
        <div class="doc-section">
            <div class="doc-section-title">Ödeme Özeti</div>
            <div class="doc-table-wrap">
                <table class="doc-table">
                    <tbody>
                        <tr>
                            <th>Ödeme Planı</th>
                            <td>${getPaymentTypeText()}</td>
                        </tr>
                        <tr>
                            <th>Sözleşme Süresi</th>
                            <td>${safe(summary.contractDurationMonths, "-")} Ay</td>
                        </tr>
                        <tr>
                            <th>İndirim Oranı</th>
                            <td>%${safe(summary.appliedDiscountRate, "0")}</td>
                        </tr>
                        <tr>
                            <th>Brüt Toplam</th>
                            <td>${formatCurrency(summary.grossTotal || 0)}</td>
                        </tr>
                        <tr>
                            <th>İndirim Tutarı</th>
                            <td>${formatCurrency(summary.discountAmount || 0)}</td>
                        </tr>
                        <tr>
                            <th>Net Toplam</th>
                            <td>${formatCurrency(summary.netTotal || 0)}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">Banka Hesap Bilgileri</div>
            <div class="doc-table-wrap">
                <table class="doc-table">
                    <tbody>
                        <tr>
                            <th>Banka Adı</th>
                            <td>Garanti Bankası</td>
                        </tr>
                        <tr>
                            <th>Hesap Sahibi</th>
                            <td>IDEIOCREATIVE Tasarım Reklamcılık Sosyal Medya Hiz. San. ve Tic. Ltd. Şti.</td>
                        </tr>
                        <tr>
                            <th>IBAN</th>
                            <td>TR11 0006 2001 2750 0006 2961 41</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        `
        );

        // 5C) Taksit planı
        if (installmentChunks.length > 0) {
            installmentChunks.forEach((chunk, index) => {
                pushPage(
                    "Ücretlendirme ve Ödeme Planı",
                    index === 0 ? "Taksit planı" : "Taksit planı devamı",
                    `
                <div class="doc-section">
                    <div class="doc-section-title">Taksit Planı${index > 0 ? " (Devam)" : ""}</div>
                    <div class="doc-table-wrap">
                        <table class="doc-table">
                            <thead>
                                <tr>
                                    <th>Dönem</th>
                                    <th>Tutar</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${buildInstallmentRows(chunk)}
                            </tbody>
                        </table>
                    </div>
                </div>
                `
                );
            });
        }

        // 6) Hizmet Veren yükümlülükleri - otomatik sayfalama
        buildListPages(
            "Hizmet Verenin Hak ve Yükümlülükleri",
            "Ajans tarafının sorumluluk ve sınırları",
            "6. Hizmet Verenin Hak ve Yükümlülükleri",
            [
                "Hizmet Veren, işbu sözleşme kapsamındaki hizmetleri mesleki bilgi ve tecrübesi doğrultusunda gerekli özeni göstererek ifa etmeyi kabul eder.",
                "Sözleşme kapsamında açıkça belirtilmeyen her türlü ek hizmet ayrıca fiyatlandırılır.",
                "Her bir video kurgusu en fazla 90 saniye ile sınırlıdır.",
                "Her video için en fazla 1 revize hakkı tanınır; ilave revizeler ayrıca fiyatlandırılır.",
                "Müzik seçimi Hizmet Alan tarafından önerilmezse Hizmet Veren tarafından seçilir; Hizmet Alan tarafından seçilen müziklerde telif sorumluluğu Hizmet Alan’a aittir.",
                "Gün içerisinde çekilen videolar aynı gün teslim edilmez; teslim süresi 3 ila 5 iş günü arasındadır.",
                "Prodüksiyonsuz paketler haricinde, Hizmet Veren tarafından gerçekleştirilmeyen çekimlere ilişkin kurgu veya düzenleme işleri ayrıca fiyatlandırılır.",
                "Çekimlerin Hizmet Alan’ın kusuru, ihmali veya ertelemesi sebebiyle yapılamaması halinde, çekim yapılmış sayılır; ancak Hizmet Alan’a ayda yalnızca bir kez erteleme hakkı tanınır.",
                "Reklam stratejisinin planlanması ve yönetimi Hizmet Veren tarafından yapılır; ancak bütçenin yetersizliği veya geç aktarılmasından doğan sonuçlardan Hizmet Veren sorumlu tutulamaz.",
                "Hizmet Veren, reklam performansı, erişim, etkileşim, satış dönüşümü, ticari kazanç veya müşteri sayısı konusunda herhangi bir garanti vermez.",
                "Hazırlanan içerik, tasarım, video ve raporlar, hizmet bedelinin tam olarak ödenmesi koşuluyla Hizmet Alan’a devredilmiş sayılır.",
                "Hizmet Veren’in, sözleşmede ayrıca yazılı şekilde kararlaştırılmadıkça gelen mesajlara cevap verme veya yorum yönetimi yükümlülüğü bulunmamaktadır."
            ],
            1,
            LIST_ITEMS_PER_PAGE
        );

        // 7) Hizmet Alan yükümlülükleri - otomatik sayfalama
        buildListPages(
            "Hizmet Alanın Hak ve Yükümlülükleri",
            "Müşteri tarafının yükümlülükleri",
            "7. Hizmet Alanın Hak ve Yükümlülükleri",
            [
                "Hizmet Alan, sözleşme kapsamında belirlenen hizmetlerin eksiksiz ve zamanında ifa edilmesini talep etme hakkına sahiptir.",
                "Hizmet Alan, iletilen içerikleri en geç 1 iş günü içinde onaylamak veya revize taleplerini bildirmekle yükümlüdür. Bu süre içinde dönüş yapılmazsa içerik onaylanmış sayılır.",
                "Hizmet Alan, sözleşme süresince gerekli veri, bilgi ve belgeleri zamanında ve eksiksiz şekilde sağlamakla yükümlüdür.",
                "Hizmet Alan, sözleşmede kararlaştırılan hizmet bedellerini tam ve zamanında ödemekle yükümlüdür.",
                "Sosyal medya hesap yönetimi hizmeti bulunması halinde, Hizmet Alan kullanıcı adı ve şifre bilgilerini Hizmet Veren’e sağlamayı kabul eder.",
                "Sosyal medya platformlarının teknik arızaları, güncellemeleri, algoritma değişiklikleri, üçüncü kişi müdahaleleri, hesap güvenliği sorunları veya Hizmet Alan kaynaklı işlemler nedeniyle oluşan zararlardan Hizmet Veren sorumlu tutulamaz.",
                "Hizmet Veren’in sorumluluğu yalnızca kendisi veya yetkilendirdiği personeli tarafından kasten ya da ağır ihmal ile yapılan işlemlerle sınırlıdır."
            ],
            1,
            LIST_ITEMS_PER_PAGE
        );

        // 8-9) Genel hükümler ilk sayfa
        buildClausePage(
            "Genel Hükümler",
            "Gizlilik ve fesih hükümleri",
            `
        <div class="doc-section">
            <div class="doc-section-title">8. Gizlilik ve Sır Saklama Yükümlülüğü</div>
            <ol class="doc-list">
                <li>Tarafların birbirleriyle paylaşacağı her türlü yazılı, sözlü, görsel, dijital veya fiziki bilgi “Gizli Bilgi” sayılır.</li>
                <li>Taraflar, gizli bilgileri yalnızca sözleşmenin amacı doğrultusunda kullanmayı ve üçüncü kişilere açıklamamayı taahhüt eder.</li>
                <li>Gizlilik yükümlülüğü, sözleşme sona erse dahi 3 yıl süreyle devam eder.</li>
                <li>Gizlilik yükümlülüğünün ihlali halinde diğer taraf sözleşmeyi haklı nedenle feshedebilir ve zararlarının tazminini talep edebilir.</li>
            </ol>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">9. Haksız Fesih ve Cezai Şart</div>
            <ol class="doc-list">
                <li>Hizmet Alan, sözleşme süresi sona ermeden ve haklı bir sebep olmaksızın sözleşmeyi feshederse, bakiye hizmet bedelinin %25’ini Hizmet Veren’e ödemeyi kabul eder.</li>
                <li>Hizmet Veren’in esaslı yükümlülüklerini üst üste 45 gün boyunca yerine getirmemesi ve yazılı ihtara rağmen 5 iş günü içinde bu eksiklikleri gidermemesi halinde, Hizmet Alan sözleşmeyi haklı nedenle feshedebilir.</li>
            </ol>
        </div>
        `
        );

        // 10-11) Genel hükümler ikinci sayfa
        buildClausePage(
            "Genel Hükümler",
            "Mücbir sebep ve uyuşmazlık çözümü",
            `
        <div class="doc-section">
            <div class="doc-section-title">10. Mücbir Sebep</div>
            <ol class="doc-list">
                <li>Doğal afet, savaş, terör, grev, salgın hastalık, internet veya enerji kesintileri, sosyal medya platformları veya üçüncü taraf servis sağlayıcılardaki kesintiler ile kamu otoriteleri kararları mücbir sebep sayılır.</li>
                <li>Mücbir sebep halinde etkilenen taraf durumu diğer tarafa yazılı olarak bildirir ve yükümlülükler askıya alınır.</li>
                <li>Mücbir sebep halinin 60 günü aşması halinde taraflardan her biri sözleşmeyi tazminatsız olarak feshedebilir.</li>
            </ol>
        </div>

        <div class="doc-section">
            <div class="doc-section-title">11. Uyuşmazlıkların Çözümü</div>
            <ol class="doc-list">
                <li>İşbu sözleşmenin uygulanmasından doğacak uyuşmazlıklarda İzmir Mahkemeleri ve İcra Daireleri yetkilidir.</li>
                <li>İşbu sözleşme taraflarca okunup kabul edilerek imza altına alınır.</li>
            </ol>
        </div>
        `
        );

        // 12) İmzalar
        buildClausePage(
            "İmzalar",
            "Tarafların kabul ve onay bölümü",
            `
        <div class="doc-section">
            <div class="doc-note">
                İşbu sözleşme taraflar arasında okunmuş, anlaşılmış ve kabul edilmiştir.
            </div>

            <div class="doc-table-wrap" style="margin-top:18px;">
                <table class="doc-table">
                    <tbody>
                        <tr>
                            <th>İmza Tarihi</th>
                            <td>${formatDate(summary.signatureDate || getContractStartDate())}</td>
                        </tr>
                        <tr>
                            <th>Hizmet Veren Yetkilisi</th>
                            <td>UĞUR FIRAT SOM</td>
                        </tr>
                        <tr>
                            <th>Hizmet Alan Yetkilisi</th>
                            <td>${safe(customer.authorizedPerson || customer.name, "Yetkili")}</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="doc-signatures">
                <div class="doc-sign-box">
                    IDEIO CREATIVE TASARIM REKLAMCILIK SOSYAL MEDYA HİZMETLERİ SANAYİ VE TİCARET LİMİTED ŞİRKETİ
                    <br><br>
                    Yetkili: UĞUR FIRAT SOM
                </div>
                <div class="doc-sign-box">
                    ${safe(customer.name, "Hizmet Alan")}
                    <br><br>
                    Yetkili: ${safe(customer.authorizedPerson || customer.name, "Yetkili")}
                </div>
            </div>
        </div>
        `
        );

        return pages;
    }

    function renderPaymentPlanSummary() {
        if (!paymentScheduleContainer) return;

        const summary = buildPaymentPlanSummary(state.selectedPaymentPlanId, state.enteredDiscountRate);

        if (!summary.plan) {
            paymentScheduleContainer.innerHTML = `<div class="empty-box">Ödeme planı bulunamadı.</div>`;
            updateHiddenInputs(summary);
            return;
        }

        if (grossTotalBox) grossTotalBox.textContent = formatCurrency(summary.grossTotal);
        if (discountAmountBox) discountAmountBox.textContent = formatCurrency(summary.discountAmount);
        if (netTotalBox) netTotalBox.textContent = formatCurrency(summary.netTotal);
        if (selectedPlanBox) {
            selectedPlanBox.textContent = `${summary.plan.name} (${summary.plan.numberOfInstallments} taksit)`;
        }
        if (contractDurationBox) {
            contractDurationBox.textContent = `${summary.contractDurationMonths} Ay`;
        }
        if (discountWarningText) {
            discountWarningText.textContent = summary.warning || "";
        }

        if (!summary.installments.length) {
            paymentScheduleContainer.innerHTML = `<div class="empty-box">Henüz dağıtılacak bir ödeme bulunmuyor.</div>`;
            updateHiddenInputs(summary);
            return;
        }

        paymentScheduleContainer.innerHTML = summary.installments.map(installment => {
       const linesHtml = installment.lines.length
           ? installment.lines.map(line => `
                    <div class="installment-line">
                        <div class="name">${escapeHtml(line.name)}</div>
                        <div class="amount">${formatCurrency(line.amount)}</div>
                    </div>
                `).join("")
           : `<div class="empty-box">Bu taksit için kalem bulunmuyor.</div>`;

       return `
                <div class="installment-row">
                    <div class="installment-head">
                        <div class="month-title">${installment.month}. Ay</div>
                        <div class="month-price">${formatCurrency(installment.net)}</div>
                    </div>
                    <div class="installment-sub">
                        Brüt: ${formatCurrency(installment.gross)} · İndirim sonrası: ${formatCurrency(installment.net)}
                    </div>
                    <div class="installment-lines">
                        ${linesHtml}
                    </div>
                </div>
            `;
   }).join("");

        updateHiddenInputs(summary);
    }


    function renderBillingPreview() {
        if (!billingPreviewArea) return;

        const customer = getCustomerFormData();
        const { items } = getSelectedItems();
        const summary = buildPaymentPlanSummary(state.selectedPaymentPlanId, state.enteredDiscountRate);

        const rawPages = state.selectedDocumentType === "contract"
   ? buildContractPages(customer, items, summary)
   : buildProposalPages(customer, items, summary);

        const docLabel = state.selectedDocumentType === "contract"
   ? "Hizmet Sözleşmesi"
   : "Ön Protokol";

        const finalPages = buildAutoPaginatedPages(rawPages, docLabel);

        billingPreviewArea.innerHTML = `<div class="document-preview-pages">${finalPages.join("")}</div>`;

        if (billingSummaryCustomer) {
            billingSummaryCustomer.textContent = customer.name || "-";
        }

        if (billingSummaryDocumentType) {
            billingSummaryDocumentType.textContent = docLabel;
        }

        if (billingSummaryPlan) {
            billingSummaryPlan.textContent = summary.plan
   ? `${summary.plan.name} (${summary.plan.numberOfInstallments} taksit)`
   : "-";
        }

        if (billingSummaryContractDuration) {
            billingSummaryContractDuration.textContent = `${summary.contractDurationMonths} Ay`;
        }

        if (billingSummaryGross) {
            billingSummaryGross.textContent = formatCurrency(summary.grossTotal);
        }

        if (billingSummaryDiscount) {
            billingSummaryDiscount.textContent = formatCurrency(summary.discountAmount);
        }

        if (billingSummaryNet) {
            billingSummaryNet.textContent = formatCurrency(summary.netTotal);
        }

        updateHiddenInputs(summary);
    }

    async function downloadBillingPdf() {
        if (isPdfDownloading) return;

        const previewPages = billingPreviewArea?.querySelectorAll(".document-preview-page");
        if (!previewPages || !previewPages.length) return;

        if (typeof window.html2canvas === "undefined" || typeof window.jspdf === "undefined") {
            alert("PDF oluşturma kütüphaneleri yüklenemedi.");
            return;
        }

        isPdfDownloading = true;

        const originalButtonText = downloadPdfBtn ? downloadPdfBtn.textContent : "";
        if (downloadPdfBtn) {
            downloadPdfBtn.disabled = true;
            downloadPdfBtn.textContent = "PDF hazırlanıyor...";
        }

        try {
            const customer = getCustomerFormData();
            const safeName = (customer.name || "teklif")
   .toLowerCase()
   .replaceAll("ı", "i")
   .replaceAll("ğ", "g")
   .replaceAll("ü", "u")
   .replaceAll("ş", "s")
   .replaceAll("ö", "o")
   .replaceAll("ç", "c")
   .replace(/[^a-z0-9]+/g, "-")
   .replace(/^-+|-+$/g, "");

            const fileName = `${safeName || "teklif"}-${state.selectedDocumentType === "contract" ? "hizmet-sozlesmesi" : "on-protokol"}.pdf`;

            await waitForImages(billingPreviewArea);

            const { jsPDF } = window.jspdf;
            const pdf = new jsPDF({
       orientation: "portrait",
       unit: "mm",
       format: "a4",
       compress: true
   });

            const pageWidth = 210;
            const pageHeight = 297;

            for (let i = 0; i < previewPages.length; i++) {
                const pageEl = previewPages[i];

                const canvas = await window.html2canvas(pageEl, {
       scale: 2,
       useCORS: true,
       backgroundColor: "#ffffff",
       scrollX: 0,
       scrollY: -window.scrollY
   });

                const imgData = canvas.toDataURL("image/jpeg", 0.98);

                if (i > 0) {
                    pdf.addPage();
                }

                pdf.addImage(imgData, "JPEG", 0, 0, pageWidth, pageHeight);
            }

            pdf.save(fileName);

            await new Promise(resolve => setTimeout(resolve, 1200));
        } catch (error) {
            console.error("PDF oluşturulurken hata:", error);
            alert("PDF oluşturulurken bir hata oluştu.");
        } finally {
            isPdfDownloading = false;

            if (downloadPdfBtn) {
                downloadPdfBtn.disabled = false;
                downloadPdfBtn.textContent = originalButtonText || "Yazdır / PDF İndir";
            }
        }
    }

    async function saveOffer() {
        if (isSavingOffer) return;

        clearSaveFeedback();

        const customer = getCustomerFormData();
        const { items } = getSelectedItems();
        const summary = buildPaymentPlanSummary(state.selectedPaymentPlanId, state.enteredDiscountRate);

        if (!customer.name) {
            showSaveFeedback("Müşteri / firma adı zorunludur.", "error");
            return;
        }

        if (!items.length) {
            showSaveFeedback("Kaydetmeden önce en az bir paket veya alt paket seçmelisiniz.", "error");
            return;
        }

        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        if (!tokenInput) {
            showSaveFeedback("Güvenlik doğrulama alanı bulunamadı.", "error");
            return;
        }

        isSavingOffer = true;
        const originalNextText = nextStepBtn ? nextStepBtn.textContent : "";

        if (nextStepBtn) {
            nextStepBtn.disabled = true;
            nextStepBtn.textContent = "Kaydediliyor...";
        }

        try {
            const formData = new FormData();
            formData.append("__RequestVerificationToken", tokenInput.value);

            formData.append("CustomerName", customer.name);
            formData.append("Email", customer.email);
            formData.append("Phone", customer.phone);
            formData.append("TaxOffice", customer.taxOffice);
            formData.append("TaxNumber", customer.taxNumber);
            formData.append("NotificationAddress", customer.address);

            formData.append("PaymentPlanId", summary.plan?.id != null ? String(summary.plan.id) : "");
            formData.append("PaymentPlanName", summary.plan?.name || "");
            formData.append("PaymentPlanInstallmentCount", summary.plan?.numberOfInstallments != null ? String(summary.plan.numberOfInstallments) : "0");
            formData.append("ContractDurationMonths", String(summary.contractDurationMonths));

            formData.append("DiscountRate", String(summary.appliedDiscountRate));
            formData.append("DiscountAmount", String(summary.discountAmount));
            formData.append("GrossTotal", String(summary.grossTotal));
            formData.append("NetTotal", String(summary.netTotal));

            formData.append("DocumentType", state.selectedDocumentType);

            formData.append("SelectedItemsJson", JSON.stringify(items.map(x => ({
       itemType: x.itemType,
       id: x.id,
       categoryId: x.categoryId,
       categoryName: x.categoryName,
       name: x.name,
       rawName: x.rawName,
       unitPrice: x.unitPrice,
       quantity: x.quantity,
       amount: x.amount,
       isOneTime: x.isOneTime,
       isPiece: x.isPiece
   }))));

            formData.append("InstallmentsJson", JSON.stringify(summary.installments.map(x => ({
       month: x.month,
       gross: x.gross,
       net: x.net
   }))));

            const response = await fetch(offerSaveUrl, {
       method: "POST",
       body: formData,
       credentials: "same-origin",
       headers: {
           "X-Requested-With": "XMLHttpRequest"
       }
   });

            const result = await response.json();

            if (!response.ok || !result.success) {
                showSaveFeedback(result.message || "Kayıt sırasında bir hata oluştu.", "error");
                return;
            }

            showSaveFeedback(result.message || "Teklif başarıyla kaydedildi.", "success");
        } catch (error) {
            console.error("Kaydetme hatası:", error);
            showSaveFeedback("Kayıt sırasında beklenmeyen bir hata oluştu.", "error");
        } finally {
            isSavingOffer = false;

            if (nextStepBtn) {
                nextStepBtn.disabled = false;
                nextStepBtn.textContent = originalNextText || "Kaydet";
            }
        }
    }

    function refreshCards() {
        document.querySelectorAll(".package-card").forEach(card => {
       const categoryId = Number(card.dataset.categoryId);
       const itemId = Number(card.dataset.itemId);
       const selected = (state.selectedPackages[categoryId] || []).includes(itemId);
       card.classList.toggle("selected", selected);

       const input = card.querySelector(".package-piece-input");
       if (input) {
           input.disabled = !selected;
       }
   });

        document.querySelectorAll(".subpackage-card").forEach(card => {
       const categoryId = Number(card.dataset.categoryId);
       const itemId = Number(card.dataset.itemId);
       const selected = (state.selectedSubPackages[categoryId] || []).includes(itemId);
       card.classList.toggle("selected", selected);

       const input = card.querySelector(".subpackage-piece-input");
       if (input) {
           input.disabled = !selected;
       }
   });

        wizardData.categories.forEach(category => {
       const packageInfo = document.getElementById(`packageSelectedInfo_${category.id}`);
       const subPackageInfo = document.getElementById(`subPackageSelectedInfo_${category.id}`);

       if (packageInfo) {
           packageInfo.textContent = `Seçilen: ${(state.selectedPackages[category.id] || []).length}`;
       }

       if (subPackageInfo) {
           subPackageInfo.textContent = `Seçilen: ${(state.selectedSubPackages[category.id] || []).length}`;
       }
   });
    }

    function refreshBottomTotal() {
        if (!bottomTotalAmount || !bottomTotalSubtext) return;

        const summary = buildPaymentPlanSummary(state.selectedPaymentPlanId, state.enteredDiscountRate);
        const currentStep = getCurrentStep();

        bottomTotalAmount.textContent = formatCurrency(summary.grossTotal);

        if (currentStep?.type === "payment" || currentStep?.type === "billing") {
            const discountText = summary.appliedDiscountRate > 0
   ? ` · İndirimli toplam: ${formatCurrency(summary.netTotal)} · Uygulanan indirim: %${summary.appliedDiscountRate}`
   : "";

            bottomTotalSubtext.textContent = `Sözleşme süresi: ${summary.contractDurationMonths} ay${discountText}`;
        } 
    }

    function refreshStepHeaders() {
        document.querySelectorAll(".wizard-step").forEach(step => {
       const stepIndex = Number(step.dataset.stepIndex);
       step.classList.remove("active", "completed");

       if (stepIndex === state.currentStep) {
           step.classList.add("active");
       } else if (stepIndex < state.currentStep) {
           step.classList.add("completed");
       }
   });
    }

    function refreshSections() {
        document.querySelectorAll(".wizard-section").forEach(section => {
       const stepIndex = Number(section.dataset.stepIndex);
       section.classList.toggle("active", stepIndex === state.currentStep);
   });
    }

    function refreshBottomButtons() {
        if (prevStepBtn) {
            prevStepBtn.disabled = state.currentStep === 0;
        }

        if (nextStepBtn) {
            nextStepBtn.disabled = false;

            if (state.currentStep === steps.length - 1) {
                nextStepBtn.textContent = "Kaydet";
            } else if (state.currentStep === steps.length - 2) {
                nextStepBtn.textContent = "Faturalandırma";
            } else {
                nextStepBtn.textContent = "İleri";
            }
        }
    }

    function goToStep(stepIndex) {
        if (stepIndex < 0 || stepIndex > steps.length - 1) return;

        state.currentStep = stepIndex;
        refreshStepHeaders();
        refreshSections();
        refreshBottomButtons();
        refreshBottomTotal();

        if (getCurrentStep()?.type === "payment") {
            renderPaymentPlanSummary();
        }

        if (getCurrentStep()?.type === "billing") {
            renderBillingPreview();
        }

        window.scrollTo({ top: 0, behavior: "smooth" });
    }

    function togglePackageSelection(categoryId, itemId) {
        const category = wizardData.categories.find(x => x.id === categoryId);
        if (!category) return;

        const list = [...(state.selectedPackages[categoryId] || [])];
        const exists = list.includes(itemId);

        if (category.isPackageMultiSelected) {
            state.selectedPackages[categoryId] = exists
   ? list.filter(x => x !== itemId)
   : [...list, itemId];
        } else {
            state.selectedPackages[categoryId] = exists ? [] : [itemId];
        }
    }

    function toggleSubPackageSelection(categoryId, itemId) {
        const category = wizardData.categories.find(x => x.id === categoryId);
        if (!category) return;

        const list = [...(state.selectedSubPackages[categoryId] || [])];
        const exists = list.includes(itemId);

        if (category.isSubPackageMultiSelected) {
            state.selectedSubPackages[categoryId] = exists
   ? list.filter(x => x !== itemId)
   : [...list, itemId];
        } else {
            state.selectedSubPackages[categoryId] = exists ? [] : [itemId];
        }
    }

    function bindEvents() {
        document.querySelectorAll(".wizard-step").forEach(step => {
       step.addEventListener("click", function() {
           const targetStep = Number(this.dataset.stepIndex);
           goToStep(targetStep);
       });
   });

        if (prevStepBtn) {
            prevStepBtn.addEventListener("click", function() {
       goToStep(state.currentStep - 1);
   });
        }

        if (nextStepBtn) {
            nextStepBtn.addEventListener("click", async function() {
       if (state.currentStep === steps.length - 1) {
           await saveOffer();
           return;
       }

       goToStep(state.currentStep + 1);
   });
        }

        document.querySelectorAll(".selection-card").forEach(card => {
       card.addEventListener("click", function(e) {
           if (e.target.closest("[data-stop-card-click='true']")) return;

           const itemType = this.dataset.itemType;
           const categoryId = Number(this.dataset.categoryId);
           const itemId = Number(this.dataset.itemId);

           if (itemType === "package") {
               togglePackageSelection(categoryId, itemId);
           } else if (itemType === "subpackage") {
               toggleSubPackageSelection(categoryId, itemId);
           }

           refreshCards();
           refreshBottomTotal();
           renderPaymentPlanSummary();
           renderBillingPreview();
       });
   });

        document.querySelectorAll(".piece-box").forEach(box => {
       box.addEventListener("click", function(e) {
           e.stopPropagation();
       });
   });

        document.querySelectorAll(".package-piece-input").forEach(input => {
       input.addEventListener("input", function() {
           const packageId = Number(this.dataset.packageId);
           let value = parseInt(this.value || "1", 10);

           if (isNaN(value) || value < 1) value = 1;

           this.value = value;
           state.selectedPackagePieces[packageId] = value;

           refreshBottomTotal();
           renderPaymentPlanSummary();
           renderBillingPreview();
       });
   });

        document.querySelectorAll(".subpackage-piece-input").forEach(input => {
       input.addEventListener("input", function() {
           const subPackageId = Number(this.dataset.subpackageId);
           let value = parseInt(this.value || "1", 10);

           if (isNaN(value) || value < 1) value = 1;

           this.value = value;
           state.subPackagePieces[subPackageId] = value;

           refreshBottomTotal();
           renderPaymentPlanSummary();
           renderBillingPreview();
       });
   });

      document.querySelectorAll(".payment-tab").forEach(tab => {
          tab.addEventListener("click", function() {
              if (this.disabled || this.classList.contains("disabled")) {
                  return;
              }

              const planId = Number(this.dataset.planId);
              const contractDurationMonths = getContractDurationMonths();
              const availablePlanIds = getAvailablePaymentPlans(contractDurationMonths).map(x => Number(x.id));

              if (!availablePlanIds.includes(planId)) {
                  return;
              }

              state.selectedPaymentPlanId = planId;

              document.querySelectorAll(".payment-tab").forEach(x => x.classList.remove("active"));
              this.classList.add("active");

              refreshBottomTotal();
              renderPaymentPlanSummary();
              renderBillingPreview();
          });
      });

        if (contractDurationInput) {
           contractDurationInput.addEventListener("input", function() {
               let value = parseInt(this.value || "1", 10);

               if (isNaN(value) || value < 1) value = 1;

               this.value = value;
               state.contractDurationMonths = value;

               syncPaymentPlanAvailability();
               renderPaymentPlanSummary();
               refreshBottomTotal();
               renderBillingPreview();
           });
       }

        if (discountRateInput) {
            discountRateInput.addEventListener("input", function() {
       let value = Number(this.value || 0);

       if (isNaN(value) || value < 0) value = 0;
       if (value > 100) value = 100;

       state.enteredDiscountRate = value;
       syncPaymentPlanAvailability();
       renderPaymentPlanSummary();
       refreshBottomTotal();
       renderBillingPreview();
   });
        }

        if (billingDocumentType) {
            billingDocumentType.addEventListener("change", function() {
       state.selectedDocumentType = this.value;
       renderBillingPreview();
   });
        }

        [
   customerNameInput,
   customerEmailInput,
   customerPhoneInput,
   customerTaxOfficeInput,
   customerTaxNumberInput,
   customerNotificationAddressInput
        ].forEach(input => {
       if (!input) return;
       input.addEventListener("input", function() {
           renderBillingPreview();
       });
   });

        if (downloadPdfBtn) {
            downloadPdfBtn.onclick = async function(e) {
                e.preventDefault();
                e.stopPropagation();

                if (isPdfDownloading) return;
                await downloadBillingPdf();
            };
        }
    }

    function init() {
        refreshCards();
        refreshStepHeaders();
        refreshSections();
        refreshBottomButtons();
        refreshBottomTotal();
        renderPaymentPlanSummary();
        renderBillingPreview();
        bindEvents();
    }

    init();
});