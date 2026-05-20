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

// TOAST MESSGAE
//window.showToast = function (message, type = 'success') {
function showToast(message, type = 'success') {
    const toastEl = document.getElementById('appToast');
    const toastBody = document.getElementById('appToastBody');

    // reset styles
    toastEl.classList.remove('text-bg-success', 'text-bg-danger');

    if (type === 'error') {
        toastEl.classList.add('text-bg-danger');

        // error → stays until manually closed
        toastEl.setAttribute('data-bs-autohide', 'false');
    } else {
        toastEl.classList.add('text-bg-success');

        // success → auto hide after delay
        toastEl.setAttribute('data-bs-autohide', 'true');
        toastEl.setAttribute('data-bs-delay', '2500'); // 2.5s 
    }

    toastBody.innerText = message;

    const toast = new bootstrap.Toast(toastEl);
    toast.show();
};

function enableValidationAutoClear(formId) {
    const form = document.getElementById(formId);
    if (!form) return; // Safety check

    // Find all input-like elements inside the form
    const inputs = form.querySelectorAll('input, select, textarea');

    // Loop through each individual input
    inputs.forEach(input => {
        ['input', 'change', 'click'].forEach(eventType => {
            input.addEventListener(eventType, () => {
                input.classList.remove('is-invalid');
                // Use form.querySelector to stay scoped to this specific form
                const errorSpan = form.querySelector(`[data-valmsg-for="${input.name}"]`);
                if (errorSpan) errorSpan.innerText = '';
            });
        });
    });
}

function showValidationErrors(errors, formId) {
    const form = document.getElementById(formId);

    // clear previous errors
    form.querySelectorAll('[data-valmsg-for]').forEach(el => el.innerText = '');
    form.querySelectorAll('.form-control').forEach(el => el.classList.remove('is-invalid'));

    for (const fieldName in errors) {
        const messages = errors[fieldName];

        // find input by name
        const input = form.querySelector(`[name="${fieldName}"]`);

        // find matching error span
        const errorSpan = form.querySelector(`[data-valmsg-for="${fieldName}"]`);

        if (input) input.classList.add('is-invalid');
        if (errorSpan) errorSpan.innerText = messages[0];
    }
}


    //form.querySelectorAll('input, textarea, select').forEach(input => {
    //    input.addEventListener('input', () => {
    //        input.classList.remove('is-invalid');

    //        const errorSpan = form.querySelector(`[data-valmsg-for="${input.name}"]`);
    //        if (errorSpan) errorSpan.innerText = '';
    //    });
    //});




