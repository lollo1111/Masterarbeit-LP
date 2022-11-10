import { createStore } from 'vuex';
import cpeeModule from './modules/cpee/index';

const store = createStore({
    modules: {
        cpee: cpeeModule
    }
});

export default store;