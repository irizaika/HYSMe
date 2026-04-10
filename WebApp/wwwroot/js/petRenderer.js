const PetRenderer = (() => {

    let services;

    function init(deps) {
        services = deps;
    }

    function renderList(pets, onClick) {
        const list = document.getElementById("nearbyList");
        list.innerHTML = "";

        pets.forEach(pet => {
            const item = document.createElement("div");

            item.className = "d-flex align-items-center px-3 py-2 border-bottom pet-row";

            item.innerHTML = `
                <img src="${pet.imageUrl || '/images/no-image.png'}"
                     class="me-3"
                     style="width:56px;height:56px;object-fit:cover;border-radius:10px;" />

                <div class="flex-grow-1">
                    <div class="fw-semibold">${pet.name}</div>
                    <div class="text-muted small">${pet.lastSeenAddress || ''}</div>
                </div>
            `;

            item.onclick = () => onClick(pet);

            list.appendChild(item);
        });
    }

    function createPopupHtml(pet) {
        return `
            <div>
                <strong>${pet.name}</strong><br/>
                ${pet.lastSeenAddress || ''}<br/>

                <button class="btn btn-sm btn-primary"
                    onclick="PetModalController.openEditModal(${pet.petId}, ${pet.isOwner})">
                    View
                </button>

                <button class="btn btn-sm btn-success"
                     onclick="SightingController.openModal(${pet.petId}, ${pet.latitude}, ${pet.longitude})">
                    👁️ Seen
                </button>
            </div>
        `;
    }

    function renderSightingsList(sightings) {
        const list = document.getElementById("sightingsList");
        list.innerHTML = `<div class="fw-semibold mb-2">Sightings</div>`;

        sightings.forEach(s => {
            const item = document.createElement("div");

            item.className = "p-2 border-bottom small";

            item.innerHTML = `
                <div>${s.dateSeen}</div>
                <div>${s.comment || ""}</div>
            `;

            item.onclick = () => {
                services.mapService.setView("modalPetMap", s.latitude, s.longitude, 16);
            };

            list.appendChild(item);
        });
    }

    return {
        init,
        renderList,
        createPopupHtml,
        renderSightingsList
    };

})();