/**
 * main.js
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Plugins
import { registerPlugins } from '@/plugins'
import VueCountdown from '@chenfengyuan/vue-countdown';

// Components
import App from './App.vue'

// Composables
import { createApp } from 'vue'

const app = createApp(App)

app.component(VueCountdown.name, VueCountdown);
registerPlugins(app)

app.mount('#app')
