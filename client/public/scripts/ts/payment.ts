import { BASE_URL } from "./config";

document.addEventListener("DOMContentLoaded", () => {
  const urlParams = new URLSearchParams(window.location.search);
  const id = urlParams.get("invoiceId");

  if (!id) {
    console.error("Invoice ID is missing.");
    // return;
  }

  const paymentContainer = document.getElementById("paymentContainer")!;
  const successMessage = document.getElementById("successMessage")!;
  const completePaymentBtn = document.getElementById("completePaymentBtn")!;

  completePaymentBtn.addEventListener("click", async () => {
    try {
      // Assume your API endpoint for completing payment
      const response = await fetch(`${BASE_URL}/payment/complete/${id}`, {
        method: "POST",
        // Add headers or other configurations if needed
      });

      if (response.ok) {
        // Payment successful
        paymentContainer.style.display = "none";
        successMessage.style.display = "block";
      } else {
        // Handle unsuccessful payment
        console.error("Payment failed");
      }
    } catch (error) {
      console.error("Error during payment:", error);
    }
  });
});
