var __require = (id) => {
  return import.meta.require(id);
};

// public/scripts/ts/config.ts
var BASE_URL = "http://localhost:5000/api";
var BASE_URL_SIGNALR = "http://localhost:5000/auctionHub";
var ADMIN_EMAIL = "test@email.com";
var ADMIN_PASSWORD = "testPassword123@";
var USER_EMAIL = "test2@hotmail.com";
var USER_PASSWORD = "testPassword123@";

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
