import { createApp } from 'vue';
import './style.css';
import App from './App.vue';
import router from './router'
import store from './store/index';
import TheCard from './components/TheCard.vue';

const app = createApp(App);
app.component('TheCard', TheCard)
app.use(router);
app.use(store);
app.mount('#app');
