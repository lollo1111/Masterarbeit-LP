<template>
    <p>Task: <strong>{{ item.task }}</strong></p>
    <br>
    <h5 @click="toggleSimulations">{{ simulationsToggled ? "🔽" : "◀️" }}Ausführende Simulationen (FIFO):</h5>
    <div v-if="simulationsToggled" class="simulations">
        <div v-if="item.items.length === 0" class="sims">
            <p>-</p>
        </div>
        <div v-else class="sims" v-for="simulation in item.items" :key="simulation.id">
            <one-simulation :simulation="simulation"></one-simulation>
        </div>
    </div>
    <br>
    <h5 @click="toggleDevices">{{ devicesToggled ? "🔽" : "◀️" }}Involvierte Geräte:</h5>
    <div v-if="devicesToggled" class="devices">
        <div v-for="device in item.devices" :key="device.id" class="devs">
            <one-device :device="device"></one-device>
        </div>
    </div>
</template>

<script>
import OneDevice from './OneDevice.vue';
import OneSimulation from './OneSimulation.vue';
export default {
    components: {
        OneDevice,
        OneSimulation
    },
    props: [
        'item'
    ],
    data() {
        return {
            simulationsToggled: false,
            devicesToggled: false
        }
    },
    methods: {
        toggleSimulations() {
            this.simulationsToggled = !this.simulationsToggled;
        },
        toggleDevices() {
            this.devicesToggled = !this.devicesToggled;
        }
    }
}
</script>

<style scoped>
h5 {
    user-select: none;
    cursor: pointer;
}

.devices,
.simulations {
    max-height: 200px;
    overflow-y: auto;
    margin: 1rem 0 0 1rem;
    background: #ccc;
    padding: 1rem;
    border-radius: 5px;
}

.sims,
.devs {
    border-bottom: 1px dotted #333;
    margin-bottom: .5rem;
    padding-bottom: .5rem;
}

.sims:last-child,
.devs:last-child {
    border-bottom: none;
    margin-bottom: 0;
    padding-bottom: 0;
}
</style>