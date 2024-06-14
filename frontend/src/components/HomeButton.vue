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
    <div class="button">
      <v-btn
        class="text-none mt-5"
        color="indigo-lighten-1"
        size="x-large"
        variant="flat"
        rounded="0"
        elevation="12"
        @click="goToChanges()"
    >
        Changes
    </v-btn>
    </div>
    <div class="button">
      <v-btn
        class="text-none mt-5"
        color="indigo-lighten-1"
        size="x-large"
        variant="flat"
        rounded="0"
        elevation="12"
        @click="goToPreferences()"
    >
        Preferences
    </v-btn>
    </div>
    <br>
</template>

<script>
  export default {
    methods: {
      goToHome() {
        this.$router.push({ name: 'Home' });
      },
      goToChanges() {
        this.$router.push({ name: 'Changes' });
      },
      goToPreferences() {
        this.$router.push({ name: 'Preferences' });
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