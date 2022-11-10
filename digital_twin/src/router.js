import { createRouter, createWebHistory } from 'vue-router';
const ExampleSite = () => import('./sites/ExampleSite.vue');

const router = createRouter({
    history: createWebHistory(), //use the build in browser support
    routes: [
        {
            path: '/',
            component: ExampleSite
        }
        // ,
        // {
        //     path: '/:notFound(.*)',
        //     component: NotFound
        // }
    ],
    linkActiveClass: 'active',
    scrollBehavior(_to, _from, savedPosition) { // underscore erlaubt es, gewisse variablen nicht benutzen zu müssen --> IDE beschwert sich so nicht
        if (savedPosition) {
            return savedPosition;
        }
    }
});

export default router;