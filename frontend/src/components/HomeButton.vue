<template>
    <div class="button">
      <v-btn
        class="text-none mt-5"
        color="indigo-lighten-1"
        size="x-large"
        variant="flat"
        rounded="0"
        elevation="12"
        @click="goToHome()"
    >
        Home
    </v-btn>
    </div>
    <br>
</template>

<script>
  export default {
    methods: {
      goToHome() {
        this.$router.push({ name: 'Home' });
      }
    }
  };
</script>

<style>
.button {
  max-width: fit-content;
  margin-left: auto;
  margin-right: auto;
}
</style>