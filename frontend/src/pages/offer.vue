<template>
  <HomeButton/>
  <div class="container">
    <!--<div class="slider"> <Slider /> </div>-->
    <div class="offer"> <OfferViewer/> </div>
  </div>
</template>

<script setup>
import OfferViewer from '../components/OfferViewer.vue';
import Slider from "../components/Slider.vue";
//
</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
body {
  margin: 0px;
}

.container {
display: grid;
grid-template-columns: 1fr;
grid-template-rows: repeat(4, 1fr);
grid-column-gap: 0px;
grid-row-gap: 0px;
}

.slider { grid-area: 1 / 1 / 2 / 6; }
.offer { grid-area: 2 / 1 / 5 / 2; }
</style>
  
  