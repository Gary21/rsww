<template>
  <vue-countdown :time=60000 :interval="100" v-slot="{ days, hours, minutes, seconds, milliseconds }">
    Time left: {{ seconds }}.{{ Math.floor(milliseconds / 100) }} seconds.
  </vue-countdown>
  <div class="button">
                  <v-btn class="text-none mt-5" color="indigo-lighten-1" size="x-large" variant="flat" rounded="0" elevation="12" @click="goToOffer(offer.id)">
                    Buy
                  </v-btn>
            </div>
</template>
<script>
//
</script>