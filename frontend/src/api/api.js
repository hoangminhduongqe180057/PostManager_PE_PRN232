import axios from "axios";

export const api = axios.create({
  baseURL: "https://postmanager-pe-prn232.onrender.com/api", // URL backend Render
});