import { defineStore } from 'pinia'

export const useInstanceStore = defineStore('InstanceStore', {
    state: () => ({ 
        worklist: [],
        instances: []
    }),
    getters: {
        getWorklist: (state) => state.worklist,
        getInstances: (state) => state.instances,
        getCpeeId: (state) => {
            return (instanceId) => state.instances.filter(instance => instance.id === instanceId)[0]["cpeeId"];
          }
    },
    actions: {
        async loadWorklist() {
            try {
                const response = await fetch('http://localhost:9033/wfms/worklist');
                const data = await response.json();
                this.worklist = data;
            } catch (error) {
                console.error(error);
            }
        },
        addInstance(instance) {
            this.instances.push(instance);
        },
        deleteInstance(instanceId) {
            this.instances = this.instances.filter(instance => instance.id !== instanceId);
        },
        setCpeeId(instanceId, cpeeId) {
            this.instances.filter(instance => instance.id === instanceId)[0]["cpeeId"] = cpeeId;
        }
    },
})