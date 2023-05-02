<template>
    <the-card>
        <template #header>Healthcheck</template>
        <template #default>
            <p v-if="stringTime">
                Zuletzt aktualisiert: {{ stringTime }}.
            </p>
            <p v-else>
                Wird aktualisiert ...
            </p>
            <ul>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :title="codes[1] === 'unhealthy' ? 'Kontrolliere, ob Factory I/O geöffnet ist.' : ''"
                                :class="{ healthy: codes[1] === 'healthy', unhealthy: codes[1] === 'unhealthy' }">{{
                                    codes[1] === "healthy" ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>Factory I/O API</h4>
                        <div class="line"></div>
                        <p>Web API der 3D-Simulation. Indem Factory I/O geöffnet wird, sowie die erforderliche
                            Konfigurationsdatei im Installationsordner hinzugefügt wurde, wird diese automatisch gestartet.
                        </p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :title="codes[11] === 'unhealthy' ? 'Kontrolliere ob SDK gestartet wurde.' : ''"
                                :class="{ healthy: codes[11] === 'healthy', unhealthy: codes[11] === 'unhealthy' }">{{
                                    codes[11] === 'healthy' ? 'HEALTHY' : 'UNHEALTHY' }}</span>
                        </div>
                        <h4>Factory I/O SDK</h4>
                        <div class="line"></div>
                        <p>Web Development Kid der 3D-Simulation. Läuft lokal auf dem Gerät und nicht in Docker. Benötigt
                            .NET Runtime sowie SDK.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :title="codes[0] === 'unhealthy' ? 'Kontrolliere Docker Microservice' : ''"
                                :class="{ healthy: codes[0] === 'healthy', unhealthy: codes[0] === 'unhealthy' }">{{
                                    codes[0] === "healthy" ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>Simulation API</h4>
                        <div class="line"></div>
                        <p>Backend Simulations-API zum Abfragen aktueller Werte sowie zur Ausführung der Simulation anhand
                            verfügbarer Endpoints.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span
                                :title="codes[2] === 'unhealthy' || codes[3] === 'unhealthy' ? 'Kontrolliere Docker Microservice. Eventuell docker-compose down -v anwenden und Docker neu starten.' : ''"
                                :class="{ healthy: codes[2] === 'healthy' && codes[3] === 'healthy', unhealthy: codes[2] === 'unhealthy' || codes[3] === 'unhealthy' }">{{
                                    codes[2] === 'healthy' && codes[3] === 'healthy' ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>WfMS (CPEE)</h4>
                        <div class="line"></div>
                        <p>Workflow Management System zur Steuerung des digitalen Zwillings. Ermöglicht die Nutzung der
                            Workflow Engine sowie eines UIs.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span
                                :title="codes[4] === 'unhealthy' ? 'Kontrolliere Docker Microservice und auf localhost:8080, ob Factory I/O erfolgreich verbunden ist.' : ''"
                                :class="{ healthy: codes[4] === 'healthy', unhealthy: codes[4] === 'unhealthy' }">{{
                                    codes[4] === "healthy" ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>PLC (OpenPLC)</h4>
                        <div class="line"></div>
                        <p>Programmable Logic Controller der digitalen Fertigungsanlage. Dient der Automatisierung indem
                            Abläufe anhand von Regeln gesteuert werden.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :title="codes[7] === 'unhealthy' ? 'Kontrolliere Docker Microservice' : ''"
                                :class="{ healthy: codes[7] === 'healthy', unhealthy: codes[7] === 'unhealthy' }">{{
                                    codes[7] === "healthy" ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>Mosquitto</h4>
                        <div class="line"></div>
                        <p>Leichtgewichtiges IoT-Kommunikationsprotokoll: verwaltet Nachrichten von IoT Geräten.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :title="codes[8] === 'unhealthy' ? 'Kontrolliere Docker Microservice' : ''"
                                :class="{ healthy: codes[8] === 'healthy', unhealthy: codes[8] === 'unhealthy' }">{{
                                    codes[8] === "healthy" ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>Kafka</h4>
                        <div class="line"></div>
                        <p>Event-Streaming Plattform: speichert und verarbeitet Datenströme.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span
                                :title="codes[9] === 'unhealthy' || codes[10] === 'unhealthy' ? 'Kontrolliere Docker Microservice.' : ''"
                                :class="{ healthy: codes[9] === 'healthy' && codes[10] === 'healthy', unhealthy: codes[9] === 'unhealthy' || codes[10] === 'unhealthy' }">{{
                                    codes[9] === 'healthy' && codes[10] === 'healthy' ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>MQTT Bridge</h4>
                        <div class="line"></div>
                        <p>Ermöglicht eine End-to-End Integration zwischen MQTT und Kafka.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span
                                :title="codes[6] === 'unhealthy' ? 'Kontrolliere in Docker, ob sowohl der Consumer als auch die Bridge aktiv sind.' : ''"
                                :class="{ healthy: codes[6] === 'healthy', unhealthy: codes[6] === 'unhealthy' }">{{
                                    codes[6] === "healthy" ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>InfluxDB</h4>
                        <div class="line"></div>
                        <p>In jener Echtzeitdatenbank werden sämtliche Daten der Simulation gespeichert. In weiterer Folge
                            können Analysen durchgeführt werden.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :title="codes[5] === 'unhealthy' ? 'Kontrolliere Docker Microservice' : ''"
                                :class="{ healthy: codes[5] === 'healthy', unhealthy: codes[5] === 'unhealthy' }">{{
                                    codes[5] === "healthy" ? "HEALTHY" : "UNHEALTHY" }}</span>
                        </div>
                        <h4>Grafana</h4>
                        <div class="line"></div>
                        <p>Ein Service, welcher die angesammelten Daten von IoT Geräten in der Echtzeitdatenbank InfluxDB in
                            Form eines Dashboards aufbereitet.</p>
                    </div>
                </li>
            </ul>
        </template>
    </the-card>
</template>

<script>
export default {
    async created() {
        try {
            do {
                this.refreshTime = null;
                const response = await fetch('http://localhost:9033/start/healthcheck');
                this.codes = ["healthy"];
                const msg = await response.json();
                for (let i = 0; i < msg.length; i++) {
                    this.codes.push(msg[i] === 200 ? "healthy" : "unhealthy");
                }
                this.refreshTime = new Date();
                await this.delay(10000);
            } while (true);

        } catch (error) {
            console.error('Error fetching data:', error);
            this.codes = ["unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy"]
        }
    },
    data() {
        return {
            refreshTime: null,
            codes: ["unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy", "unhealthy"],
        }
    },
    computed: {
        stringTime() {
            if (!this.refreshTime) return null;
            const hours = this.refreshTime.getHours().toString().padStart(2, '0');
            const minutes = this.refreshTime.getMinutes().toString().padStart(2, '0');
            const seconds = this.refreshTime.getSeconds().toString().padStart(2, '0');
            return (`${hours}:${minutes}:${seconds}`);
        }
    },
    methods: {
        delay(ms) {
            return new Promise(resolve => setTimeout(resolve, ms));
        }
    }
}
</script>

<style scoped>
.heading {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

ul {
    width: 80%;
    margin: 3rem auto;
    list-style: none;
    display: grid;
    grid-template-columns: repeat(5, 1fr);
    grid-auto-rows: 1fr;
    gap: 1.5rem;
}

ul li {
    padding: .5rem;
    border: 1px solid #000;
    border-radius: 5px;
}

ul li:hover {
    background: #ccc;
    cursor: context-menu;
    user-select: none;
}

ul li p {
    hyphens: auto;
}

h4 {
    margin-top: .5rem;
    text-align: center;
}

.healthy {
    padding: .2rem 1rem;
    background: green;
    border: none;
    border-radius: 80px;
    color: #fff;
    font-size: .8rem;
}

.unhealthy {
    font-size: .8rem;
    padding: .2rem 1rem;
    background: red;
    border: none;
    border-radius: 80px;
    color: #fff;
    cursor: help;
}

.line {
    width: 20%;
    height: 3px;
    background: #7c7a7a;
    margin: .3rem auto .2rem;
}

@media only screen and (max-width: 1200px) {
    ul {
        grid-template-columns: repeat(2, 1fr);
        width: 100%;
    }
}
</style>