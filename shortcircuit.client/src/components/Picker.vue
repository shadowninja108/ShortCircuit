<template>
	<div
		class="dropzone outer"
		@dragenter.prevent
		@dragover.prevent
		@drop="drop"
		@click="$refs.file.click()"
	>
        <div class="inner">
            <Transition>
                <span :key="label" class="text body">{{ props.label }}</span>
            </Transition>
            
            <Transition>
                <div :key="errorMessage" class="text bottom-of-parent error" >
                    {{ props.errorMessage }}
                </div>
            </Transition>
        </div>

		<input
			ref="file"
			type="file"
            :accept="accept"
			v-on:change="handleFileUpload"
			hidden
		/>
	</div>
</template>

<script setup lang="ts">
const emit = defineEmits<{
    (e: 'selected', file: File): void
}>()

const props = defineProps<{
    label: string,
    accept: string,
    errorMessage: string | undefined,
}>()

function drop(e: DragEvent) {
    e.preventDefault();
    const dataTransfer = e.dataTransfer
    if(dataTransfer === null)
        return

    const droppedFiles = dataTransfer.files
    if(droppedFiles.length != 1)
        return

    emit('selected', droppedFiles[0])
}

function handleFileUpload(e: Event) {
    const target = e.target as HTMLInputElement | null 
    if(target === null)
        return

    const uploadedFiles = target.files
    if(uploadedFiles === null)
        return
    if (uploadedFiles.length !== 1)
        return

    emit("selected", uploadedFiles[0])
}
</script>

<style scoped>
    .outer {
        width: 400px;
        height: 100%;
        padding: 10px;
    }
    .inner {
        height: 100%;
        background-color: rgb(22, 24, 25);
        border: dotted 3px grey;
        align-items: center;
        justify-content: center;
        display: flex;
        border-radius: 20px;
        cursor: pointer;
    }
    .text {
        position: absolute;
        max-width: 300px;
        text-align: center;
        width: fit-content;
        height: fit-content;
    }
    .error {
        color: red;
        padding-bottom: 20px;
    }
    .bottom-of-parent {
        position: absolute; 
        bottom: 0px;
    }

    .v-enter-active,
    .v-leave-active {
        transition: opacity 0.5s ease;
    }

    .v-enter-from,
    .v-leave-to {
        opacity: 0;
    }
</style>