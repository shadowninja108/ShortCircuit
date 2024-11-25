<template>
    <div class="center" style="bottom: 55%; text-align: center;">Wrong turn?</div>
    <div class="center" style="bottom: 50%">
        <span class="spinner">
            <span id="colon" class="inline crunch">&#65109;</span>
            <span class="inline">3</span>
        </span>
    </div>
</template>

<style scoped>
    .spinner {
        /* Haha spinny. */
        animation: spin 2s linear infinite;
    }

    .center {
        /* Center in page. */
        position: absolute;
        display: flex;
        left: 50%;
        transform: translateX(-50%) translateY(-50%);
    }

    .crunch {
        position: relative;
        /* Cut out the whitespace in the special colon char. */
        right: -7px;
        margin-left: -7px;
    }

    .inline {
        display: inline
    }

    @keyframes spin {
        0% {
            transform: rotate(0deg);
        }

        100% {
            transform: rotate(360deg);
        }
    }
</style>