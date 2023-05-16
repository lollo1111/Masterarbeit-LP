<template>
    <div class="heading">
        <div class="tex">
            <h3 @click="toggleInstance">{{ opened ? "🔽" : "◀️" }} Instanz {{ instanceId }}</h3>
        </div>
        <div class="control">
            <span @click="$emit('delete-instance', instanceId)">❌</span>
        </div>
    </div>
    <iframe v-if="opened" :src="instance" frameborder="0"></iframe>
</template>

<script>
import { useInstanceStore } from '../stores/InstanceStore';
const store = useInstanceStore();
export default {
    async created() {
        if (this.exists) {
            const cpeeId = store.getCpeeId(this.instanceId);
            this.instance = "http://localhost:8081/?monitor=http://localhost:8298/" + cpeeId + "/";
        } else if (!this.xml) {
            const formData = new URLSearchParams();
            formData.append('info', 'WfMS zur Steuerung des digitalen Zwillings');
            const response = await fetch('http://localhost:8298/', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: formData
            });
            const instance = await response.json();
            this.instance = "http://localhost:8081/?monitor=http://localhost:8298/" + instance + "/";
            store.setCpeeId(this.instanceId, instance);
        } else {
            const response = await fetch(this.url, {
                method: 'POST',
                headers: this.headers,
                body: this.xml
            });
            const msg = await response.json();
            const instance = msg["CPEE-INSTANCE"];
            this.instance = "http://localhost:8081/?monitor=http://localhost:8298/" + instance + "/";
            store.setCpeeId(this.instanceId, instance);
        }
    },
    props: [
        'instanceId',
        'xml',
        'mode',
        'exists'
    ],
    emits: [
        'delete-instance'
    ],
    data() {
        return {
            url: ("http://localhost:9296/xml?behavior=" + this.mode),
            headers: {
                'Content-Type': 'text/xml',
                'Content-ID': 'xml'
            },
            opened: true,
            instance: null
        }
    },
    methods: {
        toggleInstance() {
            this.opened = !this.opened;
        }
    }
}
</script>

<style scoped>
h3,
span,
p {
    cursor: pointer;
    user-select: none;
}

.heading {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

iframe {
    width: 100%;
    height: 100vh;
}
</style>