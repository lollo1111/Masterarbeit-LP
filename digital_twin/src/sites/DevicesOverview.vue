<template>
    <the-card>
        <template #header>Geräte Übersicht</template>
        <template #default>
            <div class="container">
                <dialog :open="filter">
                    <the-card>
                        <div class="all">
                            <div class="filter">
                                <div class="filters">
                                    <h1>Filter</h1>
                                    <a :class="{ activeFilter: filterType === 'schreibtisch' }"
                                        @click="applyFilter('schreibtisch')">{{ filterType === "schreibtisch" ? "🟢" : "🔴"
                                        }}<span>Schreibtisch-Tasks</span> </a>
                                    <a :class="{ activeFilter: filterType === 'schrank' }"
                                        @click="applyFilter('schrank')">{{
                                            filterType === "schrank" ? "🟢" : "🔴" }}<span>Schrank-Tasks</span></a>
                                    <a :class="{ activeFilter: filterType === 'both' }" @click="applyFilter('both')">{{
                                        filterType === "both" ? "🟢" : "🔴" }}<span>In beide Produkte involvierte
                                            Tasks</span></a>
                                    <a :class="{ activeFilter: filterType === 'production' }"
                                        @click="applyFilter('production')">{{ filterType === "production" ? "🟢" : "🔴"
                                        }}<span>Produktion von Möbeln</span></a>
                                    <a :class="{ activeFilter: filterType === 'logistic' }"
                                        @click="applyFilter('logistic')">{{ filterType === "logistic" ? "🟢" : "🔴"
                                        }}<span>Verpackung und Versand</span></a>
                                    <a :class="{ activeFilter: filterType === 'items' }" @click="applyFilter('items')">{{
                                        filterType === "items" ? "🟢" : "🔴" }}<span>Führt Simulation aus</span></a>
                                </div>
                                <button @click="toggleFilter">Schließen</button>
                            </div>
                        </div>
                    </the-card>
                </dialog>
                <a @click="refresh">🔄️<span class="spantext">Aktualisieren</span></a> | <a @click="toggleFilter">🎚️<span
                        class="spantext">Filter</span></a> | <a @click="toggleAuto">⏩<span
                        title="Aktualisiert automatisch alle 10 Sekunden." class="spantext">Automatisch
                        aktualisieren</span> {{ auto ? "✅" : "" }}</a>
                <p>
                    Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                    alias necessitatibus beatae perferendis, ratione placeat.
                </p>
                <ul>
                    <li :class="{ activeSimulation: item.items.length > 0 }" v-for="item of filteredDevices"
                        :key="item.task">
                        <device-group :item="item"></device-group>
                    </li>
                </ul>
            </div>
        </template>
    </the-card>
</template>

<script>
import DeviceGroup from '../components/DeviceGroup.vue';
import { useInstanceStore } from '../stores/InstanceStore';
const store = useInstanceStore();
export default {
    async created() {
        await this.refreshDevices();

    },
    components: {
        DeviceGroup
    },
    data() {
        return {
            filter: false,
            scrollPosition: 0,
            auto: false,
            filterType: null
        }
    },
    computed: {
        devices() {
            return store.getDevices;
        },
        filteredDevices() {
            if (!this.filterType) {
                return this.devices;
            } else if (this.filterType === "schreibtisch") {
                return this.devices.filter(item => item.product === "schreibtisch" || item.product === "both");
            } else if (this.filterType === "schrank") {
                return this.devices.filter(item => item.product === "schrank" || item.product === "both");
            } else if (this.filterType === "both") {
                return this.devices.filter(item => item.product === "both");
            } else if (this.filterType === "production") {
                return this.devices.filter(item => item.process === "production");
            } else if (this.filterType === "logistic") {
                return this.devices.filter(item => item.process === "logistic");
            } else if (this.filterType === "items") {
                return this.devices.filter(item => item.items.length > 0);
            }
        }
    },
    methods: {
        async refreshDevices() {
            await store.loadDevices();
        },
        toggleFilter() {
            this.filter = !this.filter;
            if (this.filter) {
                const scrollPosition = window.pageYOffset || document.documentElement.scrollTop;
                this.scrollPosition = scrollPosition;
                document.body.classList.add('modal-open');
                document.body.style.top = `-${scrollPosition}px`;
            } else {
                document.body.classList.remove('modal-open');
                window.scrollTo(0, this.scrollPosition);
            }
        },
        async toggleAuto() {
            this.auto = !this.auto;
            if (this.auto) {
                while (this.auto) {
                    await this.delay(10000);
                    await this.refreshDevices();
                    console.log("Refreshed");
                }
            }
        },
        applyFilter(filter) {
            if (this.filterType === filter) {
                this.filterType = null;
            } else {
                this.filterType = filter;
            }
        },
        delay(ms) {
            return new Promise(resolve => setTimeout(resolve, ms));
        }
    }
}
</script>

<style scoped>
.filter button {
    display: block;
    font-size: 1.2rem;
    padding: .2rem;
    margin: 0 auto;
}

.filter .filters a {
    text-decoration: underline solid 1px transparent;
    color: #000;
    cursor: pointer;
    display: block;
    margin-bottom: 1rem;
}

.filter .filters a span {
    text-decoration: underline solid 1px transparent;
}

.container {
    position: relative;
}

.all {
    /* height: 500px; */
    width: 500px;
    margin: 0 auto;
    border: 1px solid #000;
    border-radius: 5px;
    background: #fff;
}

dialog {
    position: fixed;
    top: 0;
    left: 50%;
    transform: translateX(-50%);
    width: 100%;
    height: 100%;
    z-index: 10;
    background: transparent;
    border: none;
}

dialog::after {
    content: '';
    background: rgba(255, 255, 255, 0.5);
    backdrop-filter: blur(5px);
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: -1;
}

ul {
    margin-top: 1rem;
    list-style: none;
    display: grid;
    grid-template-columns: repeat(1, 1fr);
    grid-template-rows: min-content;
    gap: 1rem;
}

ul li {
    border-radius: 5px;
    border: 1px solid #000;
    padding: .5rem;
}

.activeSimulation {
    animation: borderAnimation infinite 5s;
    border-width: 10px;
}

a {
    cursor: pointer;
}

a:hover .spantext {
    text-decoration: underline solid #000;
}

.filter {
    width: 100%;
    height: 100%;
    padding: 2rem;
    user-select: none;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
}

.filter .filters h1,
.filter button {
    text-align: center;
}

.filter .filters a.activeFilter span,
.filter .filters a:hover span {
    text-decoration-color: #000;
}

@keyframes borderAnimation {
    0% {
        border-color: red;
    }

    20% {
        border-color: blue;
    }

    60% {
        border-color: gold;
    }

    80% {
        border-color: red;
    }

    100% {
        border-color: red;
    }
}
</style>