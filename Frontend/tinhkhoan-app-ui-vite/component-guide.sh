#!/bin/bash

# Màu sắc cho output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== HƯỚNG DẪN PHÁT TRIỂN COMPONENT CHO DỰ ÁN ===${NC}"

cat << 'EOF'

# Hướng dẫn phát triển component cho TinhKhoanApp

## Cấu trúc component

```vue
<script setup lang="ts">
// 1. Imports
import { ref, computed, onMounted } from 'vue';
import type { PropType } from 'vue';
import { useRoute } from 'vue-router';
import SomeComponent from '@/components/SomeComponent.vue';

// 2. Props
const props = defineProps({
  item: {
    type: Object as PropType<Item>,
    required: true
  },
  status: {
    type: String,
    default: 'active'
  }
});

// 3. Emits
const emit = defineEmits(['update', 'delete']);

// 4. State và refs
const count = ref(0);
const inputRef = ref<HTMLInputElement | null>(null);

// 5. Computed properties
const doubleCount = computed(() => count.value * 2);

// 6. Methods
function increment() {
  count.value++;
  emit('update', count.value);
}

// 7. Lifecycle hooks
onMounted(() => {
  console.log('Component mounted');
});

// 8. Watch effects (nếu cần)
// watch(count, (newValue) => {
//   console.log(`Count changed to ${newValue}`);
// });
</script>

<template>
  <div class="component-wrapper">
    <!-- Đảm bảo sử dụng key khi dùng v-for -->
    <div v-for="(item, index) in items" :key="item.id" class="item">
      {{ item.name }}
    </div>

    <!-- Event handlers -->
    <button @click="increment">Increment</button>

    <!-- Tham chiếu template refs -->
    <input ref="inputRef" type="text" />
  </div>
</template>

<style scoped>
/* Styles chỉ áp dụng cho component này */
.component-wrapper {
  margin: 1rem;
  padding: 1rem;
  border: 1px solid #eee;
}

/* Media queries */
@media (max-width: 768px) {
  .component-wrapper {
    padding: 0.5rem;
  }
}
</style>
```

## Các nguyên tắc quan trọng

1. **Sử dụng TypeScript**: Luôn khai báo kiểu dữ liệu cho props, emits và functions

2. **Sử dụng Composition API**: Sử dụng `<script setup>` để code ngắn gọn và rõ ràng

3. **Tổ chức code**: Theo thứ tự: imports, props, emits, state, computed, methods, lifecycle hooks

4. **Tách components**: Mỗi component không nên quá 300 dòng, nếu lớn hơn hãy chia nhỏ

5. **CSS scoped**: Sử dụng `scoped` để tránh CSS conflict giữa các components

6. **Props validation**: Luôn khai báo type và default value cho props

7. **Tránh side effects**: Side effects nên được xử lý trong lifecycle hooks

8. **Tái sử dụng logic**: Tách logic phức tạp vào composables

## Ví dụ composable function

```ts
// useCounter.ts
import { ref, computed } from 'vue';

export function useCounter(initialValue = 0) {
  const count = ref(initialValue);
  const doubleCount = computed(() => count.value * 2);

  function increment() {
    count.value++;
  }

  function decrement() {
    count.value--;
  }

  return {
    count,
    doubleCount,
    increment,
    decrement
  };
}
```

## Sử dụng composable trong component

```vue
<script setup>
import { useCounter } from '@/composables/useCounter';

const { count, doubleCount, increment, decrement } = useCounter(10);
</script>

<template>
  <div>
    <p>Count: {{ count }}</p>
    <p>Double count: {{ doubleCount }}</p>
    <button @click="increment">+</button>
    <button @click="decrement">-</button>
  </div>
</template>
```

EOF

echo -e "\n${BLUE}=== KẾT THÚC HƯỚNG DẪN ===${NC}"
echo -e "${GREEN}Hãy sử dụng hướng dẫn này khi phát triển component mới.${NC}"
