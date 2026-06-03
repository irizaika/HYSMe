// Calling backend
const PetService = {
    getById: async (id) => {
        const res = await fetch(`/Pets/Get/${id}`);
        return res.json();
    },

    getByBounds: async (bounds) => {
        const url = `/Pets/GetByBounds?north=${bounds.getNorth()}&south=${bounds.getSouth()}&east=${bounds.getEast()}&west=${bounds.getWest()}`;
        const res = await fetch(url);
        return res.json();
    },

    savePet: async (formData, isEdit) => {
        return fetch(isEdit ? '/Pets/EditFromModal' : '/Pets/CreateFromModal', {
            method: isEdit ? 'PUT' : 'POST',
            body: formData
        });
    },

    createSighting: async (formData) => {
        return fetch('/Sightings/CreateSighting', {
            method: 'POST',
            body: formData
        });
    },

    getFiltered: async (search, page) => {
        const url = `/Pets/Search?search=${encodeURIComponent(search)}&page=${page}`
        const response = await fetch(url);
        const html = await response.text();
        return html; //partial view returned
    },
};