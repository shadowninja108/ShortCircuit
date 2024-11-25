import './assets/main.css'

import { createApp } from 'vue'

import App from './App.vue'

import { Quasar } from 'quasar'
import 'quasar/src/css/index.sass'
import '@quasar/extras/material-icons/material-icons.css'

import { install as VueMonacoEditorPlugin } from '@guolao/vue-monaco-editor'

import VueFileUploader from '@dafcoe/vue-file-uploader'
import '@dafcoe/vue-file-uploader/style.css'

import router from './router'

const app = createApp(App)
  .use(Quasar, { plugins: {} })
  .use(VueFileUploader)
  .use(VueMonacoEditorPlugin,
    {
      paths: {
        vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.43.0/min/vs'
      },
    })
  .use(router)

app.mount('#app')
