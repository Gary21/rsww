<template>
    <li v-for="offer in offers">
        <div class="container">
        <div class="photo"> <img :src="offer.ImgPaths[0]" alt="offer.destination" /> </div>
        <div class="date"> {{ offer.Rating }} </div>
        <div class="name"> {{ offer.Name }} </div>
        <div class="destination"> {{ offer.CityName }} </div>
        <div class="price">
            {{ offer.CountryName }}
            <div class="button">
                  <v-btn class="text-none mt-5" color="indigo-lighten-1" size="x-large" variant="flat" rounded="0" elevation="12" @click="goToOffer(offer.id)">
                    Book Now
                  </v-btn>
            </div>
        </div>
        </div>
    </li>
</template>

<script>
    export default {
    data: () => ({
        offers: [
        {
            id: 1,
            destination: 'Paris',
            name: 'Parisian Getaway',
            price: 400,
            image: 'https://letsenhance.io/static/73136da51c245e80edc6ccfe44888a99/1015f/MainBefore.jpg',
            date: '2024-09-01',
        },
        {
            id: 2,
            destination: 'Rome',
            name: 'Roman Holiday',
            price: 600,
            image: 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
            date: '2024-08-05',
        },
    ]
    }),
    methods: {
        goToOffer(id) {
        this.$router.push({
           name: 'Offer', 
           query: {
             id: id
          } 
        });
        },
        async fetchData() {
            const response = await fetch('http://localhost:8080/Offers/GetHotels');
            this.offers = await response.json();
        }
    },
    mounted() {
        this.fetchData()
    }
  };
</script>

<style>
.container {  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  grid-template-rows: 1fr 1fr;
  gap: 5px 5px;
  grid-auto-flow: row;
  grid-template-areas:
    "photo name destination"
    "photo date price";
  height: 300px;
}

.container img { width: 100%; height: 100%; object-fit: cover; }

.photo { grid-area: photo; }

.date { grid-area: date; }

.name { grid-area: name; }

.destination { grid-area: destination; }

.price {  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  grid-template-rows: 1fr 1fr 1fr;
  gap: 0px 0px;
  grid-auto-flow: row;
  grid-template-areas:
    ". . ."
    ". . ."
    ". button button";
  grid-area: price;
}

.button { grid-area: button; }

</style>