// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const FORM_MODE = {
    CREATE: 'create',
    EDIT: 'edit',
    VIEW: 'view'
};
function toggleTheme() {
    const html = document.documentElement;
    const current = html.getAttribute("data-bs-theme");

    const newTheme = current === "dark" ? "light" : "dark";

    html.setAttribute("data-bs-theme", newTheme);

    localStorage.setItem("theme", newTheme); // 

    // change visible icon, if current is light it is changes for dark - display sun
    if (current === "light") {
        iconMoon.classList.add("d-none");
        iconSun.classList.remove("d-none");
    } else {
        iconMoon.classList.remove("d-none");
        iconSun.classList.add("d-none");
    }

    const btn = document.getElementById("themeToggle");
    btn.blur(); // removes focus
}

