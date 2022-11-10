import cpeeMutations from './mutations';
import cpeeActions from './actions';
import cpeeGetters from './getters';


export default {
    namespaced: true,
    state() {
        return {
            coachID: null,
            user: null
        }
    },
    mutations: cpeeMutations,
    actions: cpeeActions,
    getters: cpeeGetters
};