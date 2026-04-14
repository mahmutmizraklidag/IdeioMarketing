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
        const plan = wizardData.paymentPlans.find(x => x.id === planId);

        if (!plan) {
            return {
                plan: null,
                grossTotal: 0,
                discountAmount: 0,
                netTotal: 0,
                appliedDiscountRate: 0,
                requestedDiscountRate: 0,
                warning: "",
                installments: []
            };
        }

        const { items, total } = getSelectedItems();
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
       const shares = splitAmountEvenly(item.amount, installmentCount);

       shares.forEach((share, index) => {
           if (share <= 0) return;

           installments[index].gross = round2(installments[index].gross + share);
           installments[index].lines.push({
               name: item.name,
               amount: share,
               type: "recurring"
           });
       });
   });

        oneTimeItems.forEach((item, index) => {
       const targetMonthIndex = Math.min(index, installmentCount - 1);
       installments[targetMonthIndex].gross = round2(installments[targetMonthIndex].gross + item.amount);
       installments[targetMonthIndex].lines.push({
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

        const grossTotal = round2(total);
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
            grossTotal,
            discountAmount,
            netTotal,
            appliedDiscountRate,
            requestedDiscountRate: safeRequestedDiscountRate,
            warning,
            installments
        };
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

        if (appliedDiscountRateInput) {
            appliedDiscountRateInput.value = summary?.appliedDiscountRate ?? 0;
        }

        if (selectedDocumentTypeInput) {
            selectedDocumentTypeInput.value = state.selectedDocumentType || "proposal";
        }
    }

    function buildItemsTableRows(items, startIndex = 0) {
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
                <td>${escapeHtml(item.rawName || item.name)}</td>
                <td>${item.quantity || 1}</td>
                <td>${formatCurrency(item.amount)}</td>
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

    function buildProposalPages(customer, items, summary) {
        const pages = [];
        const itemChunks = chunkArray(items, 12);
        const installmentChunks = chunkArray(summary.installments, 12);

        const firstItemChunk = itemChunks[0] || [];

        pages.push(`
            <div class="doc-title">Hizmet Ön Protokolü ve Teklif Formu</div>
            <div class="doc-subtitle">
                Sayın ${displayValue(customer.name, "Müşteri")} için oluşturulan teklif önizlemesi.
            </div>

            <div class="doc-section">
                <div class="doc-section-title">Müşteri Bilgileri</div>
                <div class="doc-info-grid">
                    <div class="doc-info-item">
                        <span>Müşteri / Firma Adı</span>
                        <strong>${displayValue(customer.name)}</strong>
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
                        <span>Vergi Dairesi / Vergi No</span>
                        <strong>${displayValue(customer.taxOffice)} / ${displayValue(customer.taxNumber)}</strong>
                    </div>
                    <div class="doc-info-item full">
                        <span>Tebligat Adresi</span>
                        <strong>${displayValue(customer.address)}</strong>
                    </div>
                </div>
            </div>

            <div class="doc-section">
                <div class="doc-section-title">Seçilen Hizmet Kalemleri</div>
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
                            ${buildItemsTableRows(firstItemChunk, 0)}
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
                                    ${buildItemsTableRows(itemChunks[i], i * 12)}
                                </tbody>
                            </table>
                        </div>
                    </div>
                `);
            }
        }

        const firstInstallmentChunk = installmentChunks[0] || [];
        pages.push(`
            <div class="doc-title">Ödeme Özeti</div>
            <div class="doc-subtitle">Ücretlendirme ve ödeme planı</div>

            <div class="doc-section">
                <div class="doc-section-title">Ödeme Planı Bilgileri</div>
                <div class="doc-table-wrap">
                    <table class="doc-table">
                        <tbody>
                            <tr>
                                <th>Ödeme Planı</th>
                                <td>${summary.plan ? escapeHtml(summary.plan.name) : "-"}</td>
                            </tr>
                            <tr>
                                <th>İndirim Oranı</th>
                                <td>%${summary.appliedDiscountRate}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div class="doc-total-box">
                    <div class="doc-total-row">
                        <span>Brüt Toplam</span>
                        <strong>${formatCurrency(summary.grossTotal)}</strong>
                    </div>
                    <div class="doc-total-row">
                        <span>İndirim</span>
                        <strong>${formatCurrency(summary.discountAmount)}</strong>
                    </div>
                    <div class="doc-total-row net">
                        <span>Net Toplam</span>
                        <strong>${formatCurrency(summary.netTotal)}</strong>
                    </div>
                </div>
            </div>

            <div class="doc-section">
                <div class="doc-section-title">Taksit Dağılımı</div>
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

            <div class="doc-note">
                Bu belge teklif niteliğindedir. Nihai koşullar resmi sözleşme ile kesinleşir.
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

        return pages;
    }

    function buildContractPages(customer, items, summary) {
        const pages = [];
        const itemChunks = chunkArray(items, 14);
        const installmentChunks = chunkArray(summary.installments, 12);

        pages.push(`
            <div class="doc-title">Hizmet Sözleşmesi</div>
            <div class="doc-subtitle">
                İşbu sözleşme Ideio Creative ile hizmet alan müşteri arasında düzenlenmiştir.
            </div>

            <div class="doc-section">
                <div class="doc-section-title">Taraf Bilgileri</div>
                <div class="doc-info-grid">
                    <div class="doc-info-item">
                        <span>Hizmet Veren</span>
                        <strong>Ideio Creative</strong>
                    </div>
                    <div class="doc-info-item">
                        <span>Hizmet Alan</span>
                        <strong>${displayValue(customer.name, "Müşteri / Firma")}</strong>
                    </div>
                    <div class="doc-info-item">
                        <span>E-Posta / Telefon</span>
                        <strong>${displayValue(customer.email)} / ${displayValue(customer.phone)}</strong>
                    </div>
                    <div class="doc-info-item">
                        <span>Vergi Dairesi / Vergi No</span>
                        <strong>${displayValue(customer.taxOffice)} / ${displayValue(customer.taxNumber)}</strong>
                    </div>
                    <div class="doc-info-item full">
                        <span>Tebligat Adresi</span>
                        <strong>${displayValue(customer.address)}</strong>
                    </div>
                </div>
            </div>

            <div class="doc-section">
                <div class="doc-section-title">Sözleşme Kapsamı</div>
                <ol class="doc-list">
                    <li>Hizmet Veren, aşağıda listelenen hizmetleri Hizmet Alan’a sunacaktır.</li>
                    <li>Hizmet kapsamı, seçilen paketler ve alt paketler ile sınırlıdır.</li>
                    <li>Taraflar, revize ve onay süreçlerinde yazılı iletişimi esas alacaktır.</li>
                    <li>Ödeme, seçilen ödeme planına ve indirim şartlarına göre yapılacaktır.</li>
                </ol>
            </div>
        `);

        itemChunks.forEach((chunk, index) => {
       pages.push(`
                <div class="doc-title">Seçilen Hizmetler</div>
                <div class="doc-subtitle">${index === 0 ? "Sözleşme kapsamına dahil hizmetler" : "Devam eden hizmet listesi"}</div>

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
                                ${buildItemsTableRows(chunk, index * 14)}
                            </tbody>
                        </table>
                    </div>
                </div>
            `);
   });

        const firstInstallmentChunk = installmentChunks[0] || [];
        pages.push(`
            <div class="doc-title">Ücretlendirme ve Ödeme Planı</div>
            <div class="doc-subtitle">Sözleşmeye ait ödeme özeti</div>

            <div class="doc-section">
                <div class="doc-section-title">Ödeme Bilgileri</div>
                <div class="doc-table-wrap">
                    <table class="doc-table">
                        <tbody>
                            <tr>
                                <th>Ödeme Planı</th>
                                <td>${summary.plan ? escapeHtml(summary.plan.name) : "-"}</td>
                            </tr>
                            <tr>
                                <th>İndirim Oranı</th>
                                <td>%${summary.appliedDiscountRate}</td>
                            </tr>
                            <tr>
                                <th>Brüt Toplam</th>
                                <td>${formatCurrency(summary.grossTotal)}</td>
                            </tr>
                            <tr>
                                <th>İndirim Tutarı</th>
                                <td>${formatCurrency(summary.discountAmount)}</td>
                            </tr>
                            <tr>
                                <th>Net Toplam</th>
                                <td>${formatCurrency(summary.netTotal)}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="doc-section">
                <div class="doc-section-title">Taksit Planı</div>
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
        `);

        if (installmentChunks.length > 1) {
            for (let i = 1; i < installmentChunks.length; i++) {
                pages.push(`
                    <div class="doc-title">Taksit Planı</div>
                    <div class="doc-subtitle">Devam eden taksit bilgileri</div>

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
            <div class="doc-title">Hükümler ve İmzalar</div>
            <div class="doc-subtitle">Tarafların genel mutabakat bölümü</div>

            <div class="doc-note">
                Gizlilik, fesih, mücbir sebep ve uyuşmazlık çözümü gibi detay maddeler nihai sözleşme metninde ayrıca düzenlenebilir.
            </div>

            <div class="doc-signatures">
                <div class="doc-sign-box">Hizmet Veren</div>
                <div class="doc-sign-box">${displayValue(customer.name, "Hizmet Alan")}</div>
            </div>
        `);

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

        const finalPages = rawPages.map((body, index) =>
       buildPdfPage(body, index + 1, rawPages.length, docLabel)
   );

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

        if ((currentStep?.type === "payment" || currentStep?.type === "billing") && summary.appliedDiscountRate > 0) {
            bottomTotalSubtext.textContent = `İndirimli toplam: ${formatCurrency(summary.netTotal)} · Uygulanan indirim: %${summary.appliedDiscountRate}`;
        } else {
            bottomTotalSubtext.textContent = "Seçim yaptıkça toplam burada güncellenir.";
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
           const planId = Number(this.dataset.planId);
           state.selectedPaymentPlanId = planId;

           document.querySelectorAll(".payment-tab").forEach(x => x.classList.remove("active"));
           this.classList.add("active");

           refreshBottomTotal();
           renderPaymentPlanSummary();
           renderBillingPreview();
       });
   });

        if (discountRateInput) {
            discountRateInput.addEventListener("input", function() {
       let value = Number(this.value || 0);

       if (isNaN(value) || value < 0) value = 0;
       if (value > 100) value = 100;

       state.enteredDiscountRate = value;
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