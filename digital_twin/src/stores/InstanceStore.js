import { defineStore } from 'pinia'

export const useInstanceStore = defineStore('InstanceStore', {
    state: () => ({ 
        devices: [],
        instances: []
    }),
    getters: {
        getDevices: (state) => state.devices,
        getInstances: (state) => state.instances,
        getCpeeId: (state) => {
            return (instanceId) => state.instances.filter(instance => instance.id === instanceId)[0]["cpeeId"];
          }
    },
    actions: {
        async loadDevices() {
            try {
                const response = await fetch('http://localhost:9033/wfms/devices');
                const data = await response.json();
                this.devices = data;
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