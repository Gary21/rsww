<template>
    <v-container fluid>
            <v-row align="center" justify="center">
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Destination</div>
                <v-autocomplete v-model="destination" placeholder="Destination"  :items=destinations
                          variant="outlined"></v-autocomplete>
              </v-col>
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Place of departure</div>
                <v-autocomplete v-model="departure" placeholder="Departure"  :items=departures
                          variant="outlined"></v-autocomplete>
              </v-col>
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Date of departure</div>
                <v-date-input v-model="date" variant="outlined" prepend-icon=""></v-date-input>
              </v-col>
              </v-row>
              <v-row align="center" justify="center">
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Adult</div>
                <v-number-input
                  v-model="adult"
                  :min="1"
                  :max="10"
                  :reverse="false"
                  controlVariant="split"
                  label=""
                  :hideInput="false"
                  inset
                  variant="outlined"
                ></v-number-input>
              </v-col>
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Child (11-18 years old)</div>
                <v-number-input
                  v-model="child18"
                  :min="0"
                  :max="6"
                  :reverse="false"
                  controlVariant="split"
                  label=""
                  :hideInput="false"
                  inset
                  variant="outlined"
                ></v-number-input>
              </v-col>
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Child (4-10 years old)</div>
                <v-number-input
                  v-model="child10"
                  :min="0"
                  :max="6"
                  :reverse="false"
                  controlVariant="split"
                  label=""
                  :hideInput="false"
                  inset
                  variant="outlined"
                ></v-number-input>
              </v-col>
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Child (0-3 years old)</div>
                <v-number-input
                  v-model="child3"
                  :min="0"
                  :max="6"
                  :reverse="false"
                  controlVariant="split"
                  label=""
                  :hideInput="false"
                  inset
                  variant="outlined"
                ></v-number-input>
              </v-col>
              </v-row>
              <v-row align="center" justify="center">
              <v-col cols="12" sm="2">
                  <v-btn class="text-none mt-5" color="indigo-lighten-1" size="x-large" variant="flat" rounded="0" elevation="12" v-on:click="goToOffers(destination, departure, date, adult, child18, child10, child3)">
                    Book Now
                  </v-btn>
              </v-col>
            </v-row>
          </v-container>
</template>

<script>
import { VDateInput } from 'vuetify/labs/VDateInput'
import { VNumberInput } from 'vuetify/labs/VNumberInput'

    export default {
    data: () => ({
        destinations: ['Nowy Jork', 'Madryt', 'Pekin', 'Kapsztad'],
        departures: ['Gdańsk', 'Warszawa', 'Kraków', 'Wrocław', 'Poznań', 'Katowice', 'Łódź', 'Szczecin'],
        destination: undefined,
        departure: undefined,
        date: undefined,
        adult: 1,
        child18: 0,
        child10: 0,
        child3: 0,
    }),
    methods:
    {
        goToOffers(destination, departure, date, adult, child18, child10, child3) {
        this.$router.push({
           name: 'Offers', 
           query: {
            destination: destination,
            departure: departure,
            date: date,
            adult: adult,
            child18: child18,
            child10: child10,
            child3: child3,
          } 
        });
        },
        async fetchData() {
            const response = await fetch('http://localhost:8080/Offers/GetDestinations');
            this.destinations = await response.json();
        }
    },
    mounted() {
        this.fetchData()
    },
  };
</script>
