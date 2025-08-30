# Hướng dẫn kiểm soát chất lượng code

## Giới thiệu

Tài liệu này cung cấp hướng dẫn để duy trì chất lượng code trong dự án TinhKhoanApp. Bằng cách tuân thủ các quy tắc và thực hành này, chúng ta có thể giảm thiểu lỗi, nâng cao khả năng bảo trì và đảm bảo sự nhất quán trong toàn bộ codebase.

## Các công cụ kiểm soát chất lượng

Dự án sử dụng các công cụ sau để đảm bảo chất lượng code:

1. **ESLint**: Kiểm tra lỗi cú pháp và quy tắc code
2. **TypeScript**: Kiểm tra kiểu dữ liệu
3. **Prettier**: Định dạng code tự động
4. **Custom Scripts**: Các script tùy chỉnh để phát hiện và sửa lỗi phổ biến

## Quy trình kiểm tra chất lượng

### Trước khi commit code

```bash
# Kiểm tra lỗi cú pháp và tự động sửa
npm run lint

# Kiểm tra định dạng code
npm run format

# Kiểm tra type
npm run type-check

# Kiểm tra và sửa các warning phổ biến
npm run fix-warnings

# Hoặc chạy tất cả kiểm tra một lúc
npm run quality-check
```

### Trước khi build và triển khai

```bash
# Kiểm tra type kỹ lưỡng
npm run type-check:advanced

# Build ứng dụng để đảm bảo không có lỗi
npm run build
```

## Các quy tắc lập trình

### 1. Quy tắc đặt tên

- **Components**: Sử dụng PascalCase (ví dụ: `UserProfile.vue`)
- **Files**: Sử dụng kebab-case cho files không phải component (ví dụ: `api-service.js`)
- **Variables/Functions**: Sử dụng camelCase (ví dụ: `getUserData`)
- **Constants**: Sử dụng UPPER_SNAKE_CASE (ví dụ: `MAX_RETRY_COUNT`)

### 2. Quy tắc TypeScript

- Luôn khai báo kiểu dữ liệu cho các tham số hàm và giá trị trả về
- Tránh sử dụng `any` khi có thể
- Sử dụng interface hoặc type để định nghĩa cấu trúc dữ liệu

```typescript
// Không tốt
function fetchData(url) {
  return axios.get(url)
}

// Tốt
interface UserData {
  id: number
  name: string
  email: string
}

function fetchData(url: string): Promise<UserData[]> {
  return axios.get(url)
}
```

### 3. Quy tắc Vue

- Sử dụng Composition API khi có thể
- Luôn đặt key cho v-for
- Tránh sử dụng v-html (nguy cơ XSS)
- Sử dụng emit với defineEmits để khai báo events

```vue
<script setup>
import { ref } from 'vue'

// Khai báo props và events
const props = defineProps({
  items: {
    type: Array,
    required: true,
  },
})

const emit = defineEmits(['update', 'delete'])

// State và logic
const selectedId = ref(null)

function handleSelect(id) {
  selectedId.value = id
  emit('update', id)
}
</script>

<template>
  <div>
    <ul>
      <li v-for="item in items" :key="item.id" @click="handleSelect(item.id)">
        {{ item.name }}
      </li>
    </ul>
  </div>
</template>
```

### 4. Các thực hành tốt khác

- Không để `console.log` trong code sản xuất
- Xử lý lỗi một cách rõ ràng, tránh nuốt lỗi
- Viết comment cho các đoạn code phức tạp
- Chia nhỏ component thành các phần có thể tái sử dụng
- Sử dụng các hook/composable để chia sẻ logic

## Xử lý lỗi phổ biến

### ESLint không nhận diện đúng files Vue

Nếu ESLint báo lỗi `Unexpected token <` cho files Vue, hãy kiểm tra:

1. File cấu hình ESLint (.eslintrc.cjs) đã được cấu hình đúng
2. Parser `vue-eslint-parser` đã được cài đặt và cấu hình
3. Các extensions cho ESLint đã đúng

### TypeScript báo lỗi cho các macros Vue

Nếu TypeScript báo lỗi cho các macros Vue như `defineProps`, `defineEmits`, thì:

1. Kiểm tra file tsconfig.json đã include các types cho Vue
2. Cấu hình `vueCompilerOptions` trong tsconfig.json (nếu cần)

## Tài liệu tham khảo

- [Vue Style Guide](https://vuejs.org/style-guide/)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [ESLint Documentation](https://eslint.org/docs/user-guide/getting-started)
- [Prettier Documentation](https://prettier.io/docs/en/index.html)
