// node_modules/@microsoft/sign
document.addEventListener("DOMContentLoaded", () => {
  const urlParams = new URLSearchParams(window.location.search);
  const id = urlParams.get("id");
  const paymentContainer = document.getElementById("paymentContainer");
  const successMessage = document.getElementById("successMessage");
  const completePaymentBtn = document.getElementById("completePaymentBtn");
  completePaymentBtn.addEventListener("click", async () => {
    try {
      const response = await fetch(`https://example.com/api/payment/${id}`, {
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
