import { defineStore } from 'pinia'

export const useInstanceStore = defineStore('InstanceStore', {
    state: () => ({ worklist: [] }),
    getters: {
        getWorklist: (state) => state.worklist,
    },
    actions: {
        async loadWorklist() {
            try {
                const [response1, response2] = await Promise.all([
                    fetch('http://localhost:9033/start/worklist'),
                    fetch('http://localhost:7410/api/tags')
                ]);

                const data1 = await response1.json();
                const data2 = await response2.json();
                this.worklist = data1;
            } catch (error) {
                console.error(error);
            }
        },
    },
})