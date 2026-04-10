const NavigationHelper = (() => {

    function goToPetDetails(id) {
        window.location.href = `/Pets/Details/${id}`;
    }

    function goToLogin(id) {
        window.location.href = '/Auth/Login';
    }

    return {
        goToPetDetails,
        goToLogin
    };
})();
