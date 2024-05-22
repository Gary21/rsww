<template>
    Name: {{ name }}
    <br>
    Address: {{ address }}
    <br>
    City: {{ city }}
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
    <div class="button">
                  <v-btn class="text-none mt-5" color="indigo-lighten-1" size="x-large" variant="flat" rounded="0" elevation="12" @click="goToReservation(this.$route.query.id)">
                    Reserve
                  </v-btn>
            </div>
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
        };
    },
    methods: {
        goToReservation(id) {
        this.$router.push({
           name: 'Reservation', 
           query: {
             id: id
          } 
        });
        },
        async fetchData() {
            const response = await fetch('catalog-query:5105/hotels');
            offer = await response.json()[0];
            this.name = offer.Name;
            this.address = offer.Address;
            this.city = offer.CityName;
            this.country = offer.CountryName;
            this.description = offer.Description;
            this.rating = offer.Rating;
            this.stars = offer.Stars;
            if(offer.HasFood == 1)
                this.hasFood = "yes";
            else
                this.hasFood = "no";
        }
    },
    mounted() {
        this.fetchData()
    },
    // Other component options go here
};
</script>