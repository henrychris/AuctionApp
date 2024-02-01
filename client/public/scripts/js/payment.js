"use strict";
(() => {
  // public/scripts/ts/config.ts
  var BASE_URL = "http://localhost:5000/api";

  // public/scripts/ts/payment.ts
  document.addEventListener("DOMContentLoaded", () => {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get("invoiceId");
    if (!id) {
      console.error("Invoice ID is missing.");
    }
    const paymentContainer = document.getElementById("paymentContainer");
    const successMessage = document.getElementById("successMessage");
    const completePaymentBtn = document.getElementById("completePaymentBtn");
    completePaymentBtn.addEventListener("click", async () => {
      try {
        const response = await fetch(`${BASE_URL}/payment/complete/${id}`, {
          method: "POST"
          // Add headers or other configurations if needed
        });
        if (response.ok) {
          paymentContainer.style.display = "none";
          successMessage.style.display = "block";
        } else {
          console.error("Payment failed");
        }
      } catch (error) {
        console.error("Error during payment:", error);
      }
    });
  });
})();
