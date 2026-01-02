window.blazorSubmitForm = (url, data) => {
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = url;

    // add payload fields
    for (const k in data) {
        const i = document.createElement('input');
        i.type = 'hidden'; i.name = k; i.value = data[k];
        form.appendChild(i);
    }

    // find antiforgery token input rendered by the server
    const antiInput = document.querySelector('input[name="__RequestVerificationToken"]');
    if (antiInput && antiInput.value) {
        const t = document.createElement('input');
        t.type = 'hidden';
        t.name = '__RequestVerificationToken';
        t.value = antiInput.value;
        form.appendChild(t);
    } else {
        // optional: try to read token from a known hidden element or fail loudly for debugging
        console.warn('Antiforgery token input not found on page. Ensure server rendered it.');
    }

    document.body.appendChild(form);
    form.submit();
};

