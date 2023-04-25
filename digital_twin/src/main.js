import { createApp } from 'vue';
import { createPinia } from 'pinia'
import './style.css';
import App from './App.vue';
import router from './router'
import TheCard from './components/TheCard.vue';

const app = createApp(App);
const pinia = createPinia()
app.component('TheCard', TheCard)
app.use(pinia);
app.use(router);
app.mount('#app');
