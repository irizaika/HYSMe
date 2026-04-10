const MapService = (() => {

    const maps = {};
    const layers = {};
    let sightingMarkers = [];

    function createMap(containerId, lat, lng, zoom, options = {}) {

        const {
            clickable = false,
            showMarker = false,
            onClick = null
        } = options;

        if (maps[containerId]) {
            maps[containerId].remove();
        }

        const map = L.map(containerId).setView([lat, lng], zoom);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        let marker = null;

        if (showMarker) {
            marker = L.marker([lat, lng]).addTo(map);
        }

        if (clickable) {
            map.on('click', (e) => {
                const { lat, lng } = e.latlng;

                if (marker) map.removeLayer(marker);

                marker = L.marker([lat, lng]).addTo(map);

                if (onClick) onClick(lat, lng);
            });
        }

        maps[containerId] = map;
        return map;
    }

    function getMap(id) {
        return maps[id];
    }

    function setView(id, lat, lng, zoom) {
        maps[id]?.flyTo([lat, lng], zoom);
    }

    function onMoveEnd(id, callback) {
        const map = maps[id];
        if (!map) return;

        let timeout;

        map.on('moveend', () => {
            clearTimeout(timeout);
            timeout = setTimeout(() => callback(map), 300);
        });
    }

    function getBounds(id) {
        return maps[id]?.getBounds();
    }

    function renderMarkers(id, items, createPopupHtml) {
        const map = maps[id];
        if (!map) return;

        if (layers[id]) {
            layers[id].forEach(m => map.removeLayer(m));
        }

        layers[id] = [];

        items.forEach(item => {
            const marker = L.marker([item.latitude, item.longitude])
                .addTo(map)
                .bindPopup(createPopupHtml(item));

            layers[id].push(marker);
        });
    }

    function renderSightings(sightings, mapId) {

        const html = document.documentElement;
        const current = html.getAttribute("data-bs-theme");

        const imageUrl = current === "dark" ? "/images/whitePin.png" : "/images/blackPin.png";

        const map = maps[mapId];
        if (!map) return;

        sightingMarkers.forEach(m => map.removeLayer(m));
        sightingMarkers = [];

        sightings.forEach(s => {
            const marker = L.marker([s.Latitude, s.Longitude], {
                icon: L.icon({
                    iconUrl: imageUrl,
                    iconSize: [25, 25]
                })
            })
                .addTo(map)
                .bindPopup(`
                <div>
                    <strong>${s.DateSeen}</strong><br/>
                    ${s.Comment || ""}
                </div>
            `);

            sightingMarkers.push(marker);
        });
    }

    return {
        createMap,
        getMap,
        setView,
        onMoveEnd,
        getBounds,
        renderMarkers,
        renderSightings
    };

})();