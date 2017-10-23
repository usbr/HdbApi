(function () {
    $(function () {
        var basicAuthUI =
            '<div class="input"><input placeholder="hdb" id="input_hdb" name="hdb" type="text" size="12"></div>' +
            '<div class="input"><input placeholder="username" id="input_username" name="username" type="text" size="12"></div>' +
            '<div class="input"><input placeholder="password" id="input_password" name="password" type="password" size="12"></div>';
        $(basicAuthUI).insertBefore('#api_selector div.input:last-child');
        //$("#input_baseUrl").hide();
        $("#input_apiKey").hide();

        $('#input_hdb').change(addAuthorization);
        $('#input_username').change(addAuthorization);
        $('#input_password').change(addAuthorization);
    });

function addAuthorization() {
        var hdb = $('#input_hdb').val();
        var username = $('#input_username').val();
        var password = $('#input_password').val();
        if (hdb && hdb.trim() !== "" && username && username.trim() !== "" && password && password.trim() !== "") {
            var basicAuth = new SwaggerClient.PasswordAuthorization('basic', username, password);
            window.swaggerUi.api.clientAuthorizations.add("basicAuth", basicAuth);
            window.authorizations.add("api_hdb", new SwaggerClient.ApiKeyAuthorization("api_hdb", hdb, "header"));
            window.authorizations.add("api_user", new SwaggerClient.ApiKeyAuthorization("api_user", username, "header"));
            window.authorizations.add("api_pass", new SwaggerClient.ApiKeyAuthorization("api_pass", password, "header"));
            console.log("authorization added: hdb = " + hdb + ", username = " + username + ", password = " + password);
        }
    }
})();