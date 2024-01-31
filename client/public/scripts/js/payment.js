var __require = (id) => {
  return import.meta.require(id);
};

// node_modules/@microsoft/sig
var BASE_URL = "http://localhost:5030/api";
var ADMIN_EMAIL = "test@email.com";
var ADMIN_PASSWORD = "testPassword123@";
var USER_EMAIL = "test2@hotmail.com";
var USER_PASSWORD = "testPassword123@";

// node_modules/@microsoft/sign
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
  const changeButton = document.getElementById("changeButton");
  if (changeButton) {
    changeButton.addEventListener("click", () => {
      paymentContainer.style.display = "none";
      successMessage.style.display = "block";
    });
  }
});
