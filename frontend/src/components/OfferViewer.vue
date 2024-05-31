<template>
    <v-dialog v-model="isModal">
        <template v-slot:default="{ isActive }">
            <v-card title="Error">
            <v-card-text>
                Trip for selected parameters is not available.
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

    Name: {{ name }}
    <br>
    Address: {{ address }}
    <br>
    City: {{ destination }}
    <br>
    Country: {{ country }}
    <br>
    Description: {{ description }}
    <br>
    Rating: {{ rating }}
    <br>
    Stars: {{ stars }}
    <br>
    Has food: {{ hasFood }}
    <br>

    <v-container fluid>
            <v-row align="center" justify="center">
              <v-col cols="12" sm="2">
                <div class="text-subtile-1 text-medium-emphasis">Room type</div>
                <v-autocomplete v-model="room" placeholder="RoomType"  :items=rooms
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
                  <v-btn class="text-none mt-5" color="indigo-lighten-1" size="x-large" variant="flat" rounded="0" elevation="12" v-on:click="goToReservation(this.$route.query.id, this.$route.query.departure, this.$route.query.date, this.$route.query.adult, this.$route.query.child18, this.$route.query.child10, this.$route.query.child3, this.room)">
                    Reserve
                  </v-btn>
              </v-col>
            </v-row>
          </v-container>
</template>

<script>
export default {
    name: 'OfferViewer',
    data() {
        return {
            name : 'Hotel de Ville',
            address: '123 Rue de Rivoli',
            city: 'Paris',
            country: 'France',
            description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ac',
            rating: 4.5,
            stars: 4,
            hasFood: true,
            rooms: undefined,
            room: undefined,
            destination: 'Borak',
            departures: ['Gdańsk', 'Warszawa', 'Kraków', 'Wrocław', 'Poznań', 'Katowice', 'Łódź', 'Szczecin'],
            departure: undefined,
            date: undefined,
            adult: undefined,
            child18: undefined,
            child10: undefined,
            child3: undefined,
            isModal: false
        };
    },
    methods: {
        async goToReservation(id, departure, date, adult, child18, child10, child3, room) {
        var response = await fetch('http://localhost:8080/Offers/ValidateReservation?hotelId=' + id + '&departureCity=' + departure + '&date=' + date + '&adults=' + adult + '&children18=' + child18 + '&children10=' + child10 + '&children3=' + child3 + '&roomType=' + room);
        var valid = await response.json();
        if(valid == false)
        {
            this.isModal = true;
        }
        else
        {
            var response2 = await fetch('http://localhost:8080/Offers/MakeReservation?hotelId=' + id + '&departureCity=' + departure + '&date=' + date + '&adults=' + adult + '&children18=' + child18 + '&children10=' + child10 + '&children3=' + child3 + '&roomType=' + room, {method: "POST"});
            var reservationId = await response2.json();
            this.$router.push({
                name: 'Reservation', 
                query: {
                    id: reservationId,
                } 
            });
        }},
        async fetchData() {
            const response = await fetch('http://localhost:8080/Offers/GetHotel?id=' + this.$route.query.id);
            var offer = await response.json();
            this.name = offer.name;
            this.address = offer.address;
            this.city = offer.cityName;
            this.country = offer.countryName;
            this.description = offer.description;
            this.rating = offer.rating;
            this.stars = offer.stars;
            if(offer.hasFood == 1)
                this.hasFood = "yes";
            else
                this.hasFood = "no";

            const response2 = await fetch('http://localhost:8080/Offers/GetHotelRooms?id=' + this.$route.query.id);
            this.rooms = await response2.json();
        }
    },
    mounted() {
        this.departure = this.$route.query.departure,
        this.date = this.$route.query.date,
        this.adult = this.$route.query.adult,
        this.child18 = this.$route.query.child18,
        this.child10 = this.$route.query.child10,
        this.child3 = this.$route.query.child3,
        this.fetchData()
    },
    // Other component options go here
};
</script>