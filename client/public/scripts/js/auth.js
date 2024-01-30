import { postData } from "./helper.js";
import { BASE_URL } from "./config.js";
export async function login(email, password) {
    const res = await postData(`${BASE_URL}/auth/login`, {
        emailAddress: email,
        password: password,
    });
    if (res.success) {
        return res.data;
    }
}
export async function register(firstName, lastName, email, password) {
    const role = "User";
    const res = await postData(`${BASE_URL}/auth/register`, {
        firstName: firstName,
        lastName: lastName,
        emailAddress: email,
        password: password,
        role: role,
    });
    if (res.success) {
        return res.data;
    }
}
