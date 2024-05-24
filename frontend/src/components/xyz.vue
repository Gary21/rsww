<template>
    <v-dialog v-model="isModalYes">
        <template v-slot:default="{ isActive }">
            <v-card title="Success!">
            <v-card-text>
                Trip bought successfully.
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

    <v-dialog v-model="isModalNo">
        <template v-slot:default="{ isActive }">
            <v-card title="Failed">
            <v-card-text>
                Something went wrong. Please go to homepage and try again.
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


  <vue-countdown :time=60000 :interval="100" v-slot="{ days, hours, minutes, seconds, milliseconds }">
    Time left: {{ seconds }}.{{ Math.floor(milliseconds / 100) }} seconds.
  </vue-countdown>
  <div class="button">
                  <v-btn class="text-none mt-5" color="indigo-lighten-1" size="x-large" variant="flat" rounded="0" elevation="12" @click="buyReservation(this.$route.query.id)">
                    Buy
                  </v-btn>
            </div>
</template>
<script>
export default {
    data: () => ({
        isModalYes: false,
        isModalNo: false,
    }),
    methods: {
        async buyReservation(id) {
            var response = await fetch('http://localhost:8080/Offers/BuyReservation?reservationId=' + id, {
                method: 'POST'});
            var isBought = await response.json();
            if(isBought == true)
            {
                this.isModalYes = true;
            }
            else
            {
                this.isModalNo = true;
            }
        },
    },
};
//
</script>