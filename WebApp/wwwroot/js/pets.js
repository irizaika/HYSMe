const PetModule = (() => {

    let mode = "create";
    let marker = null;
    let modalMap = null;
    let mainMap = null;
    let markersLayer = [];

    // ===== Utilities =====
    function formatDate(d) {
        const date = new Date(d);
        return date.getFullYear() + '-' +
            String(date.getMonth() + 1).padStart(2, '0') + '-' +
            String(date.getDate()).padStart(2, '0');
    }

    function getForm() {
        return document.getElementById("createPetForm");
    }

    // ===== UI helpers =====
    function setFormDisabled(isDisabled) {
        const elements = getForm().querySelectorAll("input, textarea, select");

        elements.forEach(el => {
            if (el.name !== "PetId") {
                el.disabled = isDisabled;
            }
        });
    }

    function toggleSubmitButton(isOwner) {
        const btn = document.getElementById("submit");
        if (!btn) return;

        btn.classList.toggle("d-none", !isOwner);
        btn.innerText = mode === "edit" ? "Update" : "Add";
    }

    function showPreview(url) {
        const preview = document.getElementById("preview");
        preview.src = url;
        preview.style.display = "block";
    }

    function clearPreview(url) {
        const preview = document.getElementById("preview");
        preview.src = '';
        preview.style.display = "none";
    }

    // ===== Modal Map =====
    function initMap(lat = 56.9496, lng = 24.1052, zoom = 13) {
        if (modalMap) modalMap.remove();

        modalMap = L.map('modalMap').setView([lat, lng], zoom);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(modalMap);

        marker = L.marker([lat, lng]).addTo(modalMap);

        modalMap.on('click', function (e) {
            const { lat, lng } = e.latlng;

            if (marker) modalMap.removeLayer(marker);

            marker = L.marker([lat, lng]).addTo(modalMap);

            document.getElementById("Latitude").value = lat;
            document.getElementById("Longitude").value = lng;
        });
    }

    // ===== Main Map =====

    function saveLocation(lat, lng, zoom) {
        const data = {
            lat,
            lng,
            timestamp: Date.now(),
            zoom
        };

        localStorage.setItem("userLocation", JSON.stringify(data));
    }

    function getSavedLocation() {
        const data = localStorage.getItem("userLocation");
        if (!data) return null;

        const parsed = JSON.parse(data);

        const age = Date.now() - parsed.timestamp;

        if (age > 1000 * 60 * 60) { // 1 hour
            return null;
        }

        return parsed;
    }

    function getUserLocation() {
        if (!navigator.geolocation) {
            console.log("Geolocation not supported");
            initMainMap(); // fallback
            return;
        }

        //navigator.geolocation.getCurrentPosition(
        //    (position) => {
        //        const lat = position.coords.latitude;
        //        const lng = position.coords.longitude;

        //        console.log("User location:", lat, lng);

        //        initMainMap(lat, lng);
        //    },
        //    (error) => {
        //        console.warn("Location denied or failed", error);
        //        initMainMap(); // fallback to default
        //    }
        //);

        // fast (cached)
        //navigator.geolocation.getCurrentPosition(success, error, {
        //    maximumAge: 60000,
        //    timeout: 3000,
        //    enableHighAccuracy: false
        //});

        //// then refresh silently
        //navigator.geolocation.getCurrentPosition((pos) => {
        //    mainMap.flyTo([pos.coords.latitude, pos.coords.longitude], 13);
        //}, null, {
        //    enableHighAccuracy: true
        //});

        //cached
        navigator.geolocation.getCurrentPosition(
            (position) => {
                const lat = position.coords.latitude;
                const lng = position.coords.longitude;
                const zoom = position.coords.zoom;

                saveLocation(lat, lng, zoom);

                initMainMap(lat, lng, zoom);
            },
            (error) => {
                console.warn("Location error:", error);
                initMainMap(); // fallback
            },
            {
                enableHighAccuracy: false, // faster
                //timeout: 5000,             // max 5 seconds wait
                maximumAge: 60000          // reuse cached location (1 min)
            }
        );

        // then refresh silently (not sure if needed - map pdated twice)
        //navigator.geolocation.getCurrentPosition((pos) => {
        //    mainMap.flyTo([pos.coords.latitude, pos.coords.longitude], 13);
        //}, null, {
        //    enableHighAccuracy: true
        //});

    }

    function initMainMap(lat = 56.9496, lng = 24.1052, zoom = 13)  // Riga default
    {
        if (!mainMap) {
            // map script move it
            mainMap = L.map('mainMap').setView([lat, lng], zoom);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; OpenStreetMap contributors'
            }).addTo(mainMap);

            // load pets initially
            loadPetsInView();

            let timeout;

            // reload when user moves map
            mainMap.on('moveend', (e) => {
                clearTimeout(timeout);
                timeout = setTimeout(loadPetsInView, 300);

                const center = mainMap.getCenter();

                const lat = center.lat;
                const lng = center.lng;
                const zoom = mainMap.getZoom();

                saveLocation(lat, lng, zoom);
            });
        }
        else {
            //mainMap.setView([lat, lng], 13);
            mainMap.flyTo([lat, lng], zoom);

            saveLocation(lat, lng, zoom);

            // optionally reload pets
            loadPetsInView();
        }
        
       // mainMap.on('moveend', loadPetsInView);
    }

    async function loadPetsInView() {
        const bounds = mainMap.getBounds();

        const url = `/Pets/GetByBounds?north=${bounds.getNorth()}&south=${bounds.getSouth()}&east=${bounds.getEast()}&west=${bounds.getWest()}`;

        const response = await fetch(url);
        const pets = await response.json();

        if (pets) {
            renderPets(pets);
        }
    }

    function renderPets(pets) {
        // clear old markers
        markersLayer.forEach(m => mainMap.removeLayer(m));
        markersLayer = [];

        const list = document.getElementById("nearbyList");
        list.innerHTML = "";


        pets.forEach(pet => {
            const marker = L.marker([pet.latitude, pet.longitude])
                .addTo(mainMap)
                .bindPopup(`
                <b>${pet.name}</b>
                <button class="btn btn-sm btn-outline-primary"
                    onclick="PetModule.openEditModal(${pet.petId}, false)"
                    title="View">
                    <i class="bi bi-eye"></i>
                </button><br/>
                ${pet.description}
            `);

            markersLayer.push(marker);


            //const item = document.createElement("a");
            //item.className = "list-group-item list-group-item-action d-flex gap-2";

            const item = document.createElement("div");

            item.className = "d-flex align-items-center px-3 py-2 border-bottom nearby-item pet-row";

            item.innerHTML = `
                <img src="${pet.imageUrl || '/images/no-image.png'}"
                     class="me-3"
                     style="width:50px; height:50px; object-fit:cover; border-radius:8px;" />

                <div class="flex-grow-1">
                    <div class="fw-semibold">${pet.name}</div>
                    <div class="text-muted small">${pet.lastSeenAddress || ''}</div>
                </div>
            `;

            item.onclick = () => PetModule.openEditModal(pet.petId, false);

            list.appendChild(item);

        });
    }

    //function renderPets(pets) {
    //    const list = document.getElementById("nearbyList");
    //    list.innerHTML = "";

    //    pets.forEach(pet => {
    //        const item = document.createElement("a");
    //        item.className = "list-group-item list-group-item-action d-flex gap-2";

    //        item.innerHTML = `
    //        <img src="${pet.imageUrl || '/images/no-image.png'}"
    //             style="width:50px; height:50px; object-fit:cover; border-radius:6px;" />

    //        <div>
    //            <div class="fw-semibold">${pet.name}</div>
    //            <small class="text-muted">${pet.lastSeenAddress || ''}</small>
    //        </div>
    //    `;

    //        item.onclick = () => PetModule.openEditModal(pet.petId, false);

    //        list.appendChild(item);
    //    });
    //}



    // ===== Actions =====
    function openCreateModal() {
        mode = "create";

        const form = getForm();
        form.reset();
        document.getElementById("PetId").value = "";

        setFormDisabled(false);
        toggleSubmitButton(true);

        var saved = getSavedLocation();// open modal map using main map saved location

        setTimeout(() => {
            if (saved) {
                // use instantly
                initMap(saved.lat, saved.lng, saved.zoom);
            } else {
                initMap();
            }
        }, 300);
    }

    async function openEditModal(petId, isOwner) {
        const response = await fetch(`/Pets/Get/${petId}`);
        const data = await response.json();
        const pet = data.result ?? data;

        mode = isOwner ? "edit" : "view";

        setFormDisabled(!isOwner);
        toggleSubmitButton(isOwner);

        // fill form
        document.getElementById("PetId").value = pet.petId;
        document.getElementById("Name").value = pet.name;
        document.getElementById("Type").value = pet.type;
        document.getElementById("Breed").value = pet.breed;
        document.getElementById("Color").value = pet.color;
        document.getElementById("Description").value = pet.description;
        document.getElementById("Latitude").value = pet.latitude;
        document.getElementById("Longitude").value = pet.longitude;
        document.getElementById("DateLost").value = formatDate(pet.dateLost);
        document.getElementById("Status").value = pet.status;
        document.getElementById("LastSeenAddress").value = pet.lastSeenAddress;

        new bootstrap.Modal(document.getElementById('createPetModal')).show();

        setTimeout(() => {
            initMap(pet.latitude, pet.longitude);
            if (pet.imageUrl) {
                showPreview(pet.imageUrl)
            } else {
                clearPreview();
            }
        }, 300);
    }

    async function submitPet() {
        const formData = new FormData(getForm());

        if (!formData.get("DateLost")) {
            formData.set("DateLost", new Date().toISOString());
        }

        const url = mode === "edit"
            ? '/Pets/EditFromModal'
            : '/Pets/CreateFromModal';

        const method = mode === "edit" ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method,
            body: formData
        });

        if (response.ok) {
            alert(mode === "edit" ? "Pet updated!" : "Pet created!");
            location.reload();
        } else {
            //alert("Error saving pet");
            const error = await response.json(); 
            alert("Error saving pet\n" + error.message);
        }
    }

    function initImagePreview() {
        const input = document.getElementById("ImageFile");

        if (!input) return;

        input.addEventListener("change", function (e) {
            const file = e.target.files[0];
            if (file) {
                showPreview(URL.createObjectURL(file));
            }
        });


        //const imageInput = document.getElementById("ImageFile");

        //if (imageInput) {
        //    imageInput.addEventListener("change", function (e) {
        //        const file = e.target.files[0];
        //        if (file) {
        //            const url = URL.createObjectURL(file);
        //            const preview = document.getElementById("preview");
        //            preview.src = url;
        //            preview.style.display = "block";
        //        }
        //    });

    }

    // ===== Init =====
    //function init() {
    //    initImagePreview();
    //    initMainMap();
    //}
    //function initHomePage() {
    //    initMainMap();
    //    document.addEventListener("DOMContentLoaded", () => {
    //        getUserLocation();
    //    });
    //}

    function initHomePage() {
        const saved = getSavedLocation();

        if (saved) {
            // use instantly
            initMainMap(saved.lat, saved.lng, saved.zoom);
        } else {
            initMainMap(); // fallback 
            // then refresh with real location
            getUserLocation();
        }

       
    }

    // public API
    return {
        initImagePreview,
        initHomePage,
        openCreateModal,
        openEditModal,
        submitPet,
        getUserLocation
    };

})();