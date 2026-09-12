/**
 * PetShop E-Commerce - Basket AJAX Management
 */

(function () {
    "use strict";

    // Toast bildiriş funksiyası
    function showToast(message, isSuccess = true) {
        const toastEl = document.getElementById("basketToast");
        const messageEl = document.getElementById("basketToastMessage");
        if (!toastEl || !messageEl) {
            alert(message);
            return;
        }

        toastEl.classList.remove("text-bg-success", "text-bg-danger", "text-bg-dark");
        toastEl.classList.add(isSuccess ? "text-bg-dark" : "text-bg-danger");

        const icon = isSuccess ? 'mdi:check-circle' : 'mdi:alert-circle';
        messageEl.innerHTML = `<iconify-icon icon="${icon}" class="fs-4"></iconify-icon> <span>${message}</span>`;

        if (window.bootstrap && bootstrap.Toast) {
            const toast = bootstrap.Toast.getOrCreateInstance(toastEl);
            toast.show();
        }
    }

    // Səbət UI-ni yeniləyən əsas funksiya
    function renderBasketUI(cart) {
        if (!cart) return;

        const items = cart.items || (cart.data ? cart.data.items : []) || [];
        const totalPrice = Number(cart.totalPrice !== undefined ? cart.totalPrice : (cart.data ? cart.data.totalPrice : 0)) || 0;
        const totalCount = Number(cart.totalItemCount !== undefined ? cart.totalItemCount : (cart.data ? cart.data.totalItemCount : 0)) || 0;
        const isEmpty = items.length === 0;

        const appliedCouponCode = cart.appliedCouponCode !== undefined ? cart.appliedCouponCode : (cart.data ? cart.data.appliedCouponCode : null);
        const discountAmount = Number(cart.discountAmount !== undefined ? cart.discountAmount : (cart.data ? cart.data.discountAmount : 0)) || 0;
        const discountPercent = cart.discountPercent !== undefined ? cart.discountPercent : (cart.data ? cart.data.discountPercent : null);

        // Kargo qaydası: 50+ AZN olduqda pulsuz, əks halda standart 4.99 AZN
        const isFreeShipping = cart.isFreeShipping !== undefined ? cart.isFreeShipping : (totalPrice >= 50.00 && !isEmpty);
        const shippingFee = isEmpty ? 0 : (isFreeShipping ? 0 : (cart.shippingFee !== undefined ? Number(cart.shippingFee) : 4.99));
        const amountNeeded = Number(cart.amountNeededForFreeShipping !== undefined ? cart.amountNeededForFreeShipping : Math.max(0, 50.00 - totalPrice)) || 0;
        const freeShippingProgress = Number(cart.freeShippingProgress !== undefined ? cart.freeShippingProgress : Math.min(100, (totalPrice / 50.00) * 100)) || 0;

        // Yekun məbləğ: Məhsulların cəmi - Endirim + Çatdırılma
        const finalTotal = Number(cart.finalTotal !== undefined ? cart.finalTotal : Math.max(0, totalPrice - discountAmount + shippingFee)) || 0;

        const formattedSubtotal = totalPrice.toFixed(2) + " \u20BC";
        const formattedFinalTotal = finalTotal.toFixed(2) + " \u20BC";
        const formattedDiscount = "-" + discountAmount.toFixed(2) + " \u20BC";
        const formattedShipping = isFreeShipping ? "Pulsuz (-4.99 \u20BC)" : shippingFee.toFixed(2) + " \u20BC";

        // 1) Navbar və başlıq sayğacları
        document.querySelectorAll(".cart-badge-count").forEach(el => {
            el.innerText = totalCount;
        });

        // 2) Offcanvas UI yeniləmə
        const offcanvasCount = document.getElementById("CartItemCount");
        if (offcanvasCount) offcanvasCount.innerText = totalCount;

        const offcanvasEmptyMsg = document.getElementById("CartEmptyMessage");
        if (offcanvasEmptyMsg) {
            offcanvasEmptyMsg.classList.toggle("d-none", !isEmpty);
        }

        const offcanvasFooter = document.getElementById("CartFooterSection");
        if (offcanvasFooter) {
            offcanvasFooter.classList.toggle("d-none", isEmpty);
        }



        // Offcanvas Məhsul siyahısı
        const offcanvasList = document.getElementById("CartItems");
        if (offcanvasList) {
            offcanvasList.classList.toggle("d-none", isEmpty);
            offcanvasList.innerHTML = "";

            items.forEach(item => {
                const li = document.createElement("li");
                li.className = "list-group-item py-3 px-0 border-bottom";
                li.setAttribute("data-cart-item-id", item.cartItemId);

                const priceChangedBadge = item.hasPriceChanged
                    ? `<span class="badge bg-warning text-dark" style="font-size: 0.7rem;">Qiym\u0259t d\u0259yi\u015Fib!</span>`
                    : "";

                const variantBadge = item.variantLabel
                    ? `<small class="text-muted d-block text-truncate">${item.variantLabel}</small>`
                    : "";

                let stockUrgencyHtml = "";
                if (item.availableStock > 0 && item.availableStock < 5) {
                    stockUrgencyHtml = `
                        <div class="text-danger small fw-bold mt-1 d-flex align-items-center">
                            <iconify-icon icon="mdi:fire" class="me-1"></iconify-icon> Alma\u011Fa t\u0259l\u0259sin! C\u0259mi ${item.availableStock} \u0259d\u0259d qald\u0131!
                        </div>
                    `;
                } else if (item.availableStock === 0) {
                    stockUrgencyHtml = `
                        <div class="text-danger small fw-bold mt-1 d-flex align-items-center">
                            <iconify-icon icon="mdi:alert-circle" class="me-1"></iconify-icon> T\u00FCk\u0259nib!
                        </div>
                    `;
                }

                li.innerHTML = `
                    <div class="d-flex align-items-center gap-3">
                        <img src="${item.productImagePath}" alt="${item.productName}" style="width: 55px; height: 55px; object-fit: cover;" class="rounded border flex-shrink-0" onerror="this.onerror=null;this.src='/images/no-image.png';">
                        <div class="flex-grow-1 min-w-0">
                            <a href="/product/${item.productId}" class="text-decoration-none text-dark">
                                <h6 class="my-0 text-truncate" title="${item.productName}">${item.productName}</h6>
                            </a>
                            ${variantBadge}
                            ${priceChangedBadge}
                            <div class="small fw-medium text-dark mt-1">${Number(item.unitPrice).toFixed(2)}\u20BC</div>
                            ${stockUrgencyHtml}
                        </div>
                        <div class="text-end flex-shrink-0">
                            <div class="fw-bold text-dark mb-1">${Number(item.totalPrice).toFixed(2)}\u20BC</div>
                            <button type="button" class="btn btn-link text-danger p-0 border-0 fs-6 text-decoration-none" onclick="removeFromCart(${item.cartItemId})" title="Sil">
                                <iconify-icon icon="mdi:trash-can-outline"></iconify-icon>
                            </button>
                        </div>
                    </div>
                    <div class="d-flex align-items-center justify-content-between mt-2 pt-2 border-top border-light">
                        <span class="small text-dark fw-medium">Say:</span>
                        <div class="qty-stepper d-inline-flex align-items-center justify-content-center border rounded-2 bg-white shadow-sm" style="height: 30px;">
                            <button type="button" class="btn btn-link text-dark p-0 d-flex align-items-center justify-content-center border-0"
                                    style="width: 26px; height: 100%; text-decoration: none;"
                                    onclick="updateQuantity(${item.cartItemId}, ${item.quantity - 1})"
                                    title="Azalt">
                                <iconify-icon icon="mdi:minus" style="font-size: 0.8rem;"></iconify-icon>
                            </button>
                            <input type="text" class="form-control border-0 text-center p-0 fw-bold text-dark"
                                   style="width: 32px; height: 100%; box-shadow: none; background: transparent; font-size: 0.85rem;"
                                   readonly value="${item.quantity}">
                            <button type="button" class="btn btn-link text-dark p-0 d-flex align-items-center justify-content-center border-0"
                                    style="width: 26px; height: 100%; text-decoration: none;"
                                    onclick="updateQuantity(${item.cartItemId}, ${item.quantity + 1})"
                                    title="Art\u0131r">
                                <iconify-icon icon="mdi:plus" style="font-size: 0.8rem;"></iconify-icon>
                            </button>
                        </div>
                    </div>
                `;
                offcanvasList.appendChild(li);
            });
        }

        // Offcanvas Kupon Bölməsi
        const offcanvasCouponSection = document.getElementById("CartCouponSection");
        if (offcanvasCouponSection) {
            offcanvasCouponSection.classList.toggle("d-none", isEmpty);
        }

        const cartCouponContainer = document.getElementById("CartCouponContainer");
        if (cartCouponContainer) {
            if (appliedCouponCode) {
                const discountText = discountPercent ? `%${discountPercent} endirim` : `${discountAmount.toFixed(2)} \u20BC endirim`;
                cartCouponContainer.innerHTML = `
                    <div class="d-flex align-items-center justify-content-between p-2 rounded border" style="background-color: #e8f5e9; border-color: #a5d6a7 !important;" id="CartCouponAppliedBox">
                        <div class="d-flex align-items-center gap-2">
                            <span class="badge text-white fw-bold px-2 py-1" style="background-color: #2e7d32; font-size: 0.8rem;" id="CartAppliedCouponBadge">${appliedCouponCode}</span>
                            <span class="fw-bold small" style="color: #1b5e20;" id="CartAppliedCouponText">${discountText}</span>
                        </div>
                        <button type="button" class="btn btn-sm btn-outline-danger py-0 px-2 fw-semibold" onclick="removeCoupon()" title="Kuponu l\u0259\u011Fv et">
                            <iconify-icon icon="mdi:close" class="align-middle"></iconify-icon> Sil
                        </button>
                    </div>
                `;
            } else {
                cartCouponContainer.innerHTML = `
                    <div id="CartCouponInputBox">
                        <div class="input-group input-group-sm">
                            <input type="text" class="form-control text-uppercase ps-3 text-dark fw-medium" 
                                   placeholder="Kupon kodu daxil edin" 
                                   id="CartCouponCodeInput"
                                   style="font-size: 0.85rem; letter-spacing: 0.5px;">
                            <button class="btn btn-dark px-3 text-nowrap" 
                                    type="button" 
                                    onclick="applyCoupon('CartCouponCodeInput')"
                                    style="font-size: 0.8rem; font-weight: 500;">
                                T\u0259tbiq et
                            </button>
                        </div>
                    </div>
                `;
            }
        }

        const cartCouponMsg = document.getElementById("CartCouponMessage");
        if (cartCouponMsg) {
            cartCouponMsg.classList.add("d-none");
            cartCouponMsg.innerText = "";
        }

        // Offcanvas Qiymət Xülasəsi (Açılıb-bağlanan təfərrüat bölməsi)
        const cartSubtotal = document.getElementById("CartSubtotalPrice");
        if (cartSubtotal) cartSubtotal.innerText = formattedSubtotal;

        const cartShippingDisplay = document.getElementById("CartShippingDisplay");
        if (cartShippingDisplay) {
            if (isFreeShipping) {
                cartShippingDisplay.innerText = "Pulsuz \uD83C\uDF89";
                cartShippingDisplay.style.color = "#2e7d32";
                cartShippingDisplay.className = "small fw-bold";
            } else {
                cartShippingDisplay.innerText = isEmpty ? "0.00 \u20BC" : (shippingFee.toFixed(2) + " \u20BC");
                cartShippingDisplay.style.color = "";
                cartShippingDisplay.className = "small fw-bold text-dark";
            }
        }

        const cartShippingHintContainer = document.getElementById("CartShippingHintContainer");
        if (cartShippingHintContainer) {
            if (!isFreeShipping && !isEmpty && amountNeeded > 0) {
                cartShippingHintContainer.innerHTML = `
                    <div class="small fst-italic mb-2" style="font-size: 0.78rem; color: #6c757d;">
                        Pulsuz \u00E7atd\u0131r\u0131lma \u00FC\u00E7\u00FCn daha <strong class="text-primary">${amountNeeded.toFixed(2)} \u20BC</strong> \u0259lav\u0259 edin
                    </div>
                `;
            } else {
                cartShippingHintContainer.innerHTML = "";
            }
        }

        const cartCouponDiscountContainer = document.getElementById("CartCouponDiscountContainer");
        if (cartCouponDiscountContainer) {
            if (appliedCouponCode && discountAmount > 0) {
                cartCouponDiscountContainer.innerHTML = `
                    <div class="d-flex justify-content-between align-items-center mb-2 pt-1 border-top border-light">
                        <span class="small fw-semibold" style="color: #2e7d32;">
                            <iconify-icon icon="mdi:tag" class="align-middle me-1"></iconify-icon>${appliedCouponCode} kuponu:
                        </span>
                        <span class="small fw-bold" style="color: #2e7d32;">-${discountAmount.toFixed(2)} \u20BC</span>
                    </div>
                `;
            } else {
                cartCouponDiscountContainer.innerHTML = "";
            }
        }

        const cartDetailFinalPrice = document.getElementById("CartDetailFinalPrice");
        if (cartDetailFinalPrice) cartDetailFinalPrice.innerText = formattedFinalTotal;

        const cartFinalPrice = document.getElementById("CartFinalPrice");
        if (cartFinalPrice) cartFinalPrice.innerText = formattedFinalTotal;

        // 3) Səbət səhifəsi (/Basket/Index) UI yeniləmə
        const basketEmptyContainer = document.getElementById("BasketEmptyContainer");
        const basketContentContainer = document.getElementById("BasketContentContainer");
        const basketTableBody = document.getElementById("BasketTableBody");

        if (basketEmptyContainer && basketContentContainer) {
            basketEmptyContainer.classList.toggle("d-none", !isEmpty);
            basketContentContainer.classList.toggle("d-none", isEmpty);
        }



        // Index Kupon Bölməsi
        const indexCouponAppliedBox = document.getElementById("IndexCouponAppliedBox");
        const indexCouponInputBox = document.getElementById("IndexCouponInputBox");
        const indexAppliedBadge = document.getElementById("IndexAppliedCouponBadge");
        const indexAppliedText = document.getElementById("IndexAppliedCouponText");
        const indexCouponInput = document.getElementById("IndexCouponCodeInput");
        const indexCouponMsg = document.getElementById("IndexCouponMessage");

        if (indexCouponMsg) {
            indexCouponMsg.classList.add("d-none");
            indexCouponMsg.innerText = "";
        }

        if (appliedCouponCode) {
            if (indexCouponAppliedBox) {
                indexCouponAppliedBox.classList.remove("d-none");
                indexCouponAppliedBox.style.backgroundColor = "#e8f5e9";
                indexCouponAppliedBox.style.borderColor = "#a5d6a7";
            }
            if (indexCouponInputBox) indexCouponInputBox.classList.add("d-none");
            if (indexAppliedBadge) {
                indexAppliedBadge.innerText = appliedCouponCode;
                indexAppliedBadge.style.backgroundColor = "#2e7d32";
            }
            if (indexAppliedText) {
                indexAppliedText.innerText = discountPercent ? `%${discountPercent} endirim` : `${discountAmount.toFixed(2)} \u20BC endirim`;
                indexAppliedText.style.color = "#1b5e20";
                indexAppliedText.className = "fw-bold small";
            }
        } else {
            if (indexCouponAppliedBox) indexCouponAppliedBox.classList.add("d-none");
            if (indexCouponInputBox) indexCouponInputBox.classList.remove("d-none");
            if (indexCouponInput) indexCouponInput.value = "";
        }

        // Index Cədvəl Məhsulları
        if (basketTableBody) {
            basketTableBody.innerHTML = "";
            items.forEach(item => {
                const tr = document.createElement("tr");
                tr.id = `basket-row-${item.cartItemId}`;
                tr.className = "border-bottom";

                const priceChangedWarning = item.hasPriceChanged
                    ? `<div class="text-warning small mt-1"><iconify-icon icon="mdi:alert-circle-outline"></iconify-icon> Cari ma\u011Faza qiym\u0259ti: ${Number(item.currentPrice).toFixed(2)}\u20BC</div>`
                    : "";

                const variantBadge = item.variantLabel
                    ? `<span class="badge bg-light text-dark border me-1">${item.variantLabel}</span>`
                    : "";

                let indexStockUrgencyHtml = "";
                if (item.availableStock > 0 && item.availableStock < 5) {
                    indexStockUrgencyHtml = `
                        <div class="text-danger small fw-bold mt-2 d-flex align-items-center justify-content-center">
                            <iconify-icon icon="mdi:fire" class="me-1 fs-5"></iconify-icon> Alma\u011Fa t\u0259l\u0259sin! C\u0259mi ${item.availableStock} \u0259d\u0259d qald\u0131!
                        </div>
                    `;
                } else if (item.availableStock === 0) {
                    indexStockUrgencyHtml = `
                        <div class="text-danger small fw-bold mt-2 d-flex align-items-center justify-content-center">
                            <iconify-icon icon="mdi:alert-circle" class="me-1 fs-5"></iconify-icon> T\u00FCk\u0259nib!
                        </div>
                    `;
                }

                tr.innerHTML = `
                    <td class="py-4">
                        <div class="d-flex align-items-center">
                            <img src="${item.productImagePath}" alt="${item.productName}"
                                 class="rounded-3 border me-3"
                                 style="width: 80px; height: 80px; object-fit: cover;"
                                 onerror="this.onerror=null;this.src='/images/no-image.png';">
                            <div>
                                <h5 class="card-title mb-1">
                                    <a href="/product/${item.productId}" class="text-decoration-none text-dark">${item.productName}</a>
                                </h5>
                                ${variantBadge}
                                <div class="text-dark small mt-1">
                                    Qiym\u0259t: <strong class="text-dark">${Number(item.unitPrice).toFixed(2)}\u20BC</strong>
                                </div>
                                ${priceChangedWarning}
                            </div>
                        </div>
                    </td>
                    <td class="py-4 text-center">
                        <div class="qty-stepper d-inline-flex align-items-center justify-content-center border rounded-2 bg-white shadow-sm" style="height: 36px;">
                            <button type="button" class="btn btn-link text-dark p-0 d-flex align-items-center justify-content-center border-0"
                                    style="width: 30px; height: 100%; text-decoration: none;"
                                    onclick="updateQuantity(${item.cartItemId}, ${item.quantity - 1})"
                                    title="Azalt">
                                <iconify-icon icon="mdi:minus" class="fs-6"></iconify-icon>
                            </button>
                            <input type="number" min="1" max="${item.quantity + item.availableStock}"
                                   value="${item.quantity}"
                                   class="form-control border-0 text-center p-0 fw-bold qty-input"
                                   style="width: 36px; height: 100%; box-shadow: none; background: transparent; font-size: 0.95rem;"
                                   id="qty-input-${item.cartItemId}"
                                   onchange="updateQuantity(${item.cartItemId}, parseInt(this.value) || 1)">
                            <button type="button" class="btn btn-link text-dark p-0 d-flex align-items-center justify-content-center border-0"
                                    style="width: 30px; height: 100%; text-decoration: none;"
                                    onclick="updateQuantity(${item.cartItemId}, ${item.quantity + 1})"
                                    title="Art\u0131r">
                                <iconify-icon icon="mdi:plus" class="fs-6"></iconify-icon>
                            </button>
                        </div>
                        ${indexStockUrgencyHtml}
                    </td>
                    <td class="py-4 text-end">
                        <span class="fs-5 fw-bold text-dark" id="subtotal-${item.cartItemId}">
                            ${Number(item.totalPrice).toFixed(2)}\u20BC
                        </span>
                    </td>
                    <td class="py-4 text-center">
                        <button type="button" class="btn btn-link text-danger p-0 border-0"
                                onclick="removeFromCart(${item.cartItemId})"
                                title="Sil">
                            <iconify-icon icon="mdi:trash-can-outline" class="fs-4"></iconify-icon>
                        </button>
                    </td>
                `;
                basketTableBody.appendChild(tr);
            });
        }

        // Index Xülasə Cədvəli
        const indexSubtotal = document.getElementById("IndexSubtotal");
        if (indexSubtotal) indexSubtotal.innerText = formattedSubtotal;

        const indexShippingStandard = document.getElementById("IndexShippingStandard");
        if (indexShippingStandard) {
            indexShippingStandard.innerText = isEmpty ? "0.00 \u20BC" : "4.99 \u20BC";
        }

        const indexFreeShippingRow = document.getElementById("IndexFreeShippingDiscountRow");
        if (indexFreeShippingRow) {
            indexFreeShippingRow.classList.toggle("d-none", !isFreeShipping);
        }

        const indexFreeShippingDiscount = document.getElementById("IndexFreeShippingDiscount");
        if (indexFreeShippingDiscount) {
            indexFreeShippingDiscount.innerText = "-4.99 \u20BC";
        }

        const indexDiscountRow = document.getElementById("IndexDiscountRow");
        const indexDiscountAmount = document.getElementById("IndexDiscountAmount");
        const indexDiscountLabel = document.getElementById("IndexDiscountLabel");
        if (indexDiscountRow) indexDiscountRow.classList.toggle("d-none", discountAmount <= 0);
        if (indexDiscountAmount) indexDiscountAmount.innerText = formattedDiscount;
        if (indexDiscountLabel) {
            indexDiscountLabel.innerText = appliedCouponCode ? `\u00DCmumi endirim (${appliedCouponCode}):` : "\u00DCmumi endirim:";
        }

        // YEKUN MƏBLƏĞ: Kargo və endirim daxil olmaqla dəqiq hesablanır
        const indexTotalPrice = document.getElementById("IndexTotalPrice");
        if (indexTotalPrice) indexTotalPrice.innerText = formattedFinalTotal;
    }

    // Səhifədəki stok göstəricilərini və düymələri canlı yeniləyən funksiya
    function updateStockUI(productId, remainingStock) {
        if (productId === undefined || productId === null) return;

        const stockElements = document.querySelectorAll(`.stock-count[data-product-id="${productId}"]`);

        let newStock = remainingStock;
        if (newStock === undefined || newStock === null) {
            if (stockElements.length > 0) {
                const currentVal = parseInt(stockElements[0].innerText) || 0;
                newStock = Math.max(0, currentVal - 1);
            }
        }

        if (newStock !== undefined && newStock !== null) {
            stockElements.forEach(el => {
                el.innerText = newStock;
                el.classList.add("text-warning");
                el.style.transform = "scale(1.2)";
                el.style.display = "inline-block";
                el.style.transition = "transform 0.2s ease, color 0.2s ease";
                setTimeout(() => {
                    el.classList.remove("text-warning");
                    el.style.transform = "scale(1)";
                }, 300);
            });

            if (newStock <= 0) {
                const stockCell = document.getElementById(`stock-cell-${productId}`);
                if (stockCell) {
                    stockCell.innerHTML = `
                        <span class="badge bg-danger text-white px-3 py-2 fw-semibold fs-6 shadow-sm d-inline-flex align-items-center">
                            <iconify-icon icon="mdi:close-circle" class="me-1 fs-6"></iconify-icon> T\u00FCk\u0259nib
                        </span>
                    `;
                }

                const actionWrapper = document.getElementById(`action-wrapper-${productId}`);
                if (actionWrapper) {
                    const addBtn = actionWrapper.querySelector(".add-to-cart-btn");
                    if (addBtn) {
                        const disabledBtn = document.createElement("button");
                        disabledBtn.className = "btn btn-secondary px-4 py-2 text-uppercase fs-6 rounded-1 text-nowrap disabled";
                        disabledBtn.setAttribute("disabled", "disabled");
                        disabledBtn.innerText = "T\u00FCk\u0259nib";
                        addBtn.replaceWith(disabledBtn);
                    }
                }

                document.querySelectorAll(`a.btn-cart[onclick*="AddToBasket(${productId})"], a.btn-cart[onclick*="addToBasket(${productId})"]`).forEach(btn => {
                    const disabledBtn = document.createElement("button");
                    disabledBtn.className = "btn btn-secondary flex-grow-1 py-2 text-uppercase fs-6 rounded-1 disabled";
                    disabledBtn.setAttribute("disabled", "disabled");
                    disabledBtn.innerText = "T\u00FCk\u0259nib";
                    btn.replaceWith(disabledBtn);
                });
            } else {
                const stockCell = document.getElementById(`stock-cell-${productId}`);
                if (stockCell && stockCell.querySelector(".bg-danger")) {
                    stockCell.innerHTML = `
                        <span class="badge bg-success text-white px-3 py-2 fw-semibold fs-6 shadow-sm d-inline-flex align-items-center" style="background-color: #198754 !important;">
                            <iconify-icon icon="mdi:check-circle" class="me-1 fs-6"></iconify-icon> Stokda var (<span class="stock-count" data-product-id="${productId}">${newStock}</span>)
                        </span>
                    `;
                }
            }
        }
    }

    // 1) Səbətə əlavə etmək (arxa planda yenilənir, offcanvas avtomatik açılmır)
    async function addToBasket(productId, variantId = null, quantity = 1) {
        try {
            const response = await fetch("/Basket/AddToCart", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    productId: parseInt(productId),
                    productVariantId: variantId ? parseInt(variantId) : null,
                    quantity: parseInt(quantity) || 1
                })
            });

            const data = await response.json();

            if (response.ok && data.success) {
                renderBasketUI(data);
                updateStockUI(productId, data.remainingStock);
                showToast(data.message || "M\u0259hsul s\u0259b\u0259t\u0259 \u0259lav\u0259 edildi!", true);
            } else {
                showToast(data.message || "X\u0259ta ba\u015F verdi.", false);
            }
        } catch (err) {
            console.error("AddToCart error:", err);
            showToast("\u018Flaq\u0259 x\u0259tas\u0131 ba\u015F verdi.", false);
        }
    }

    // 2) Səbətdəki məhsul sayını dəyişmək
    async function updateQuantity(cartItemId, newQuantity) {
        try {
            const response = await fetch("/Basket/UpdateQuantity", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    cartItemId: parseInt(cartItemId),
                    quantity: parseInt(newQuantity)
                })
            });

            const data = await response.json();

            if (response.ok && data.success) {
                renderBasketUI(data);
                showToast(data.message || "S\u0259b\u0259t yenil\u0259ndi.", true);
            } else {
                showToast(data.message || "X\u0259ta ba\u015F verdi.", false);
                loadBasket();
            }
        } catch (err) {
            console.error("UpdateQuantity error:", err);
            showToast("\u018Flaq\u0259 x\u0259tas\u0131 ba\u015F verdi.", false);
        }
    }

    // 3) Səbətdən silmək
    async function removeFromCart(cartItemId) {
        try {
            const response = await fetch("/Basket/RemoveFromCart", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    cartItemId: parseInt(cartItemId)
                })
            });

            const data = await response.json();

            if (response.ok && data.success) {
                renderBasketUI(data);
                if (data.productId && data.remainingStock !== undefined) {
                    updateStockUI(data.productId, data.remainingStock);
                }
                showToast(data.message || "M\u0259hsul s\u0259b\u0259td\u0259n \u00E7\u0131xar\u0131ld\u0131.", true);
            } else {
                showToast(data.message || "X\u0259ta ba\u015F verdi.", false);
            }
        } catch (err) {
            console.error("RemoveFromCart error:", err);
            showToast("\u018Flaq\u0259 x\u0259tas\u0131 ba\u015F verdi.", false);
        }
    }

    // 4) Səhifə yüklənəndə cari səbəti gətirmək
    async function loadBasket() {
        try {
            const response = await fetch("/Basket/GetBasket");
            if (response.ok) {
                const data = await response.json();
                renderBasketUI(data);
            }
        } catch (err) {
            console.error("GetBasket error:", err);
        }
    }

    // 5) Endirim kuponunu tətbiq etmək
    async function applyCoupon(inputId) {
        let code = "";
        if (inputId) {
            const inputEl = document.getElementById(inputId);
            if (inputEl) code = inputEl.value.trim();
        } else {
            const cartInput = document.getElementById("CartCouponCodeInput");
            const indexInput = document.getElementById("IndexCouponCodeInput");
            code = (cartInput && cartInput.value.trim()) || (indexInput && indexInput.value.trim()) || "";
        }

        if (!code) {
            showToast("Z\u0259hm\u0259t olmasa kupon kodunu daxil edin.", false);
            return;
        }

        try {
            const response = await fetch("/Basket/ApplyCoupon", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ code: code })
            });

            const data = await response.json();

            if (response.ok && data.success) {
                renderBasketUI(data);
                showToast(data.message || "Kupon u\u011Furla t\u0259tbiq edildi!", true);
            } else {
                showToast(data.message || "Kupon etibars\u0131zd\u0131r.", false);
                const cartMsg = document.getElementById("CartCouponMessage");
                if (cartMsg) {
                    cartMsg.innerText = data.message;
                    cartMsg.className = "small mt-1 text-danger d-block";
                }
                const indexMsg = document.getElementById("IndexCouponMessage");
                if (indexMsg) {
                    indexMsg.innerText = data.message;
                    indexMsg.className = "small mt-1 text-danger d-block";
                }
            }
        } catch (err) {
            console.error("ApplyCoupon error:", err);
            showToast("\u018Flaq\u0259 x\u0259tas\u0131 ba\u015F verdi.", false);
        }
    }

    // 6) Endirim kuponunu ləğv etmək
    async function removeCoupon() {
        try {
            const response = await fetch("/Basket/RemoveCoupon", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            });

            const data = await response.json();

            if (response.ok && data.success) {
                renderBasketUI(data);
                showToast(data.message || "Kupon l\u0259\u011Fv edildi.", true);
            } else {
                showToast(data.message || "X\u0259ta ba\u015F verdi.", false);
            }
        } catch (err) {
            console.error("RemoveCoupon error:", err);
            showToast("\u018Flaq\u0259 x\u0259tas\u0131 ba\u015F verdi.", false);
        }
    }

    window.addToBasket = addToBasket;
    window.AddToBasket = addToBasket;
    window.updateQuantity = updateQuantity;
    window.removeFromCart = removeFromCart;
    window.loadBasket = loadBasket;
    window.updateStockUI = updateStockUI;
    window.applyCoupon = applyCoupon;
    window.removeCoupon = removeCoupon;

    document.addEventListener("DOMContentLoaded", function () {
        loadBasket();

        // Kupon input-larında Enter düyməsini dinləmək
        document.addEventListener("keypress", function (e) {
            if (e.key === "Enter") {
                if (e.target && e.target.id === "CartCouponCodeInput") {
                    e.preventDefault();
                    applyCoupon("CartCouponCodeInput");
                } else if (e.target && e.target.id === "IndexCouponCodeInput") {
                    e.preventDefault();
                    applyCoupon("IndexCouponCodeInput");
                }
            }
        });

        // Səbət breakdown ox işarəsinin (chevron) fırlanması
        const breakdownCollapse = document.getElementById("CartBreakdownDetails");
        const collapseChevron = document.getElementById("CartCollapseChevron");
        if (breakdownCollapse && collapseChevron) {
            breakdownCollapse.addEventListener("show.bs.collapse", function () {
                collapseChevron.style.transform = "rotate(180deg)";
            });
            breakdownCollapse.addEventListener("hide.bs.collapse", function () {
                collapseChevron.style.transform = "rotate(0deg)";
            });
        }
    });
})();