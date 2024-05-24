<template>
        <v-container fluid>
            <v-row align="center" justify="center">
              <v-col cols="12" sm="2">
                <v-text-field v-model="username" label="Username"></v-text-field>
              </v-col>
              <v-col cols="12" sm="2">
                <v-text-field v-model="password" label="Password"></v-text-field>
              </v-col>
              </v-row>
              <v-row align="center" justify="center">
              <v-col cols="12" sm="2">
                  <v-btn class="text-none mt-5" color="indigo-lighten-1" size="x-large" variant="flat" rounded="0" elevation="12" v-on:click="login(username, password)">
                    Login
                  </v-btn>
              </v-col>
            </v-row>
        </v-container>

        <v-dialog v-model="isModal">
        <template v-slot:default="{ isActive }">
            <v-card title="Failed">
            <v-card-text>
                Failed to login! Please try again.
            </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>

                <v-btn
                text="Close"
                @click="isActive.value = false"
                ></v-btn>
            </v-card-actions>
            </v-card>
        </template>
    </v-dialog>
</template>

<script>
export default {
    data: () => ({
        username: '',
        password: '',
        isModal: false
    }),
    methods: {
        async login(username, password) {
            var response = await fetch('http://localhost:8080/Offers/Login?username=' + username + '&password=' + password, {
                method: 'POST'});
            var isLogged = await response.json();
            if(isLogged != -1)
            {
                this.$router.push({name: 'Home'});
            }
            else
            {
                this.isModal = true;
            }
        }
    }
};
</script>