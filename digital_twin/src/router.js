import { createRouter, createWebHistory } from 'vue-router';
const TheStartsite = () => import('./sites/TheStartsite.vue');
const TheDashboard = () => import('./sites/TheDashboard.vue');
const TheForm = () => import('./sites/TheForm.vue');
const TheWfms = () => import('./sites/TheWfms.vue');
const TheWorklist = () => import('./sites/TheWorklist.vue');
const HealthCheck = () => import('./sites/HealthCheck.vue');
const NotFound = () => import('./sites/NotFound.vue');

const router = createRouter({
    history: createWebHistory(), //use the build in browser support
    routes: [
        {
            path: '/',
            component: TheStartsite
        }, {
            path: '/dashboard',
            component: TheDashboard
        }, {
            path: '/wfms',
            component: TheWfms
        }, {
            path: '/form',
            component: TheForm
        }, {
            path: '/worklist',
            component: TheWorklist
        }, {
            path: '/healthcheck',
            component: HealthCheck
        }, {
            path: '/:notFound(.*)',
            component: NotFound
        }
    ],
    linkActiveClass: 'active',
    scrollBehavior(_to, _from, savedPosition) { // underscore erlaubt es, gewisse variablen nicht benutzen zu müssen --> IDE beschwert sich so nicht
        if (savedPosition) {
            return savedPosition;
        }
    }
});

export default router;