const LostPetPageController = (() => {

    let services;
    let searchTimeout;

    function init(deps) {
        services = deps;


            document.getElementById("petSearch").addEventListener("input", function() {
            clearTimeout(searchTimeout);

            searchTimeout = setTimeout(() => { loadPetsPage(1); }, 300);
        });
    }
    
    // same on home page
    function handleReportClick() {
        if (!services.isAuthenticated) {
            services.navigationHelper.goToLogin();
            return;
        }
        services.petModalController.openCreateModal();
    }
    


    async function loadPetsPage(page = 1) {
        const search = document.getElementById("petSearch").value;
        const html = await PetService.getFiltered(search, page);

        document.getElementById("updatePetListContainer").innerHTML = html;
    }

    return {
        init,
        handleReportClick
    };
})();