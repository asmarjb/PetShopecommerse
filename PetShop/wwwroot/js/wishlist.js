/**
 * PetShop E-Commerce - Wishlist AJAX Management
 */

(function () {
    "use strict";

    function showToast(message, isSuccess = true) {
        const toastEl = document.getElementById("basketToast");
        const messageEl = document.getElementById("basketToastMessage");
        if (toastEl && messageEl) {
            toastEl.classList.remove("text-bg-success", "text-bg-danger", "text-bg-dark");
            toastEl.classList.add(isSuccess ? "text-bg-dark" : "text-bg-danger");
            const icon = isSuccess ? "mdi:heart" : "mdi:alert-circle";
            messageEl.innerHTML = `<iconify-icon icon="${icon}" class="fs-4" style="color: #ff0000;"></iconify-icon> <span>${message}</span>`;
            if (window.bootstrap && bootstrap.Toast) {
                const toast = bootstrap.Toast.getOrCreateInstance(toastEl);
                toast.show();
            }
            return;
        }
        alert(message);
    }

    // Change 1: Toggle wishlist — adds/removes and flips .wishlisted class + title
    async function toggleWishlist(btnEl, productId) {
        const isCurrentlyWishlisted = btnEl.classList.contains("wishlisted");

        if (isCurrentlyWishlisted) {
            // Remove from wishlist
            try {
                const response = await fetch("/Wishlist/RemoveFromWishlist", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ productId: parseInt(productId) })
                });
                const data = await response.json();
                if (response.ok && data.success) {
                    btnEl.classList.remove("wishlisted", "text-danger", "border-danger");
                    btnEl.style.borderColor = "";
                    btnEl.style.color = "#000000";
                    btnEl.blur();
                    const icon = btnEl.querySelector("iconify-icon");
                    if (icon) {
                        icon.classList.remove("text-danger");
                        icon.style.setProperty("color", "#000000", "important");
                    }
                    btnEl.title = "İstək siyahısına əlavə et";
                    showToast(data.message || "Məhsul istək siyahısından çıxarıldı.", true);
                } else {
                    showToast(data.message || "Xəta baş verdi.", false);
                }
            } catch (err) {
                console.error("RemoveFromWishlist error:", err);
                showToast("Əlaqə xətası baş verdi.", false);
            }
        } else {
            // Add to wishlist
            try {
                const response = await fetch("/Wishlist/AddToWishlist", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ productId: parseInt(productId), productVariantId: null })
                });
                const data = await response.json();
                if (response.ok && data.success) {
                    btnEl.classList.add("wishlisted");
                    btnEl.style.borderColor = "#ff0000";
                    btnEl.style.color = "#ff0000";
                    btnEl.blur();
                    const icon = btnEl.querySelector("iconify-icon");
                    if (icon) {
                        icon.style.setProperty("color", "#ff0000", "important");
                    }
                    btnEl.title = "İstək siyahısından çıxar";
                    showToast(data.message || "Məhsul istək siyahısına əlavə edildi!", true);
                } else {
                    showToast(data.message || "Xəta baş verdi.", false);
                }
            } catch (err) {
                console.error("AddToWishlist error:", err);
                showToast("Əlaqə xətası baş verdi.", false);
            }
        }
    }

    async function addToWishlist(productId, variantId = null) {
        try {
            const response = await fetch("/Wishlist/AddToWishlist", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    productId: parseInt(productId),
                    productVariantId: variantId ? parseInt(variantId) : null
                })
            });
            const data = await response.json();
            if (response.ok && data.success) {
                showToast(data.message || "Məhsul istək siyahısına əlavə edildi!", true);
            } else {
                showToast(data.message || "Xəta baş verdi.", false);
            }
        } catch (err) {
            console.error("AddToWishlist error:", err);
            showToast("Əlaqə xətası baş verdi.", false);
        }
    }

    async function removeFromWishlist(wishlistItemId, productId = null) {
        try {
            const response = await fetch("/Wishlist/RemoveFromWishlist", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    wishlistItemId: wishlistItemId ? parseInt(wishlistItemId) : null,
                    productId: productId ? parseInt(productId) : null
                })
            });
            const data = await response.json();
            if (response.ok && data.success) {
                showToast(data.message || "Məhsul istək siyahısından çıxarıldı.", true);

                // Remove the wishlist table row with animation
                const row = document.getElementById(`wishlist-row-${wishlistItemId}`);
                if (row) {
                    row.style.transition = "opacity 0.3s ease, transform 0.3s ease";
                    row.style.opacity = "0";
                    row.style.transform = "translateX(20px)";
                    setTimeout(() => {
                        row.remove();
                        const tbody = document.getElementById("WishlistTableBody");
                        if (tbody && tbody.children.length === 0) {
                            const tableContainer = document.getElementById("WishlistTableContainer");
                            const emptyContainer = document.getElementById("WishlistEmptyContainer");
                            if (tableContainer) tableContainer.classList.add("d-none");
                            if (emptyContainer) emptyContainer.classList.remove("d-none");
                        }
                    }, 300);
                }
            } else {
                showToast(data.message || "Xəta baş verdi.", false);
            }
        } catch (err) {
            console.error("RemoveFromWishlist error:", err);
            showToast("Əlaqə xətası baş verdi.", false);
        }
    }

    window.toggleWishlist = toggleWishlist;
    window.addToWishlist = addToWishlist;
    window.AddToWishlist = addToWishlist;
    window.removeFromWishlist = removeFromWishlist;
    window.RemoveFromWishlist = removeFromWishlist;
})();
