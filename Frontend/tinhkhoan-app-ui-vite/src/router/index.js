import { createRouter, createWebHistory } from "vue-router";
import { isAuthenticated } from "../services/auth";

// ⚡ ENHANCED LAZY LOADING - Preload critical routes and group by feature
import HomeView from "../views/HomeView.vue";
import LoginView from "../views/LoginView.vue";
// ✅ Sử dụng file mới với bảng nghiệp vụ đầy đủ và template hợp lệ
import DataImportView from "../views/DataImportViewFull.vue";

// Preload critical components (optional for faster navigation)
const preloadCriticalComponents = () => {
  // Preload commonly accessed views
  import(/* webpackChunkName: "critical" */ "../views/EmployeesView.vue");
  import(/* webpackChunkName: "critical" */ "../views/KpiScoringView.vue");
  import(/* webpackChunkName: "critical" */ "../views/UnitsView.vue");
};

// Call preload after initial load
setTimeout(preloadCriticalComponents, 2000);

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeView,
  },
  {
    path: "/login",
    name: "login",
    component: LoginView,
  },
  // 🔍 Debug route for testing stores
  {
    path: "/store-debug",
    name: "store-debug",
    component: () => import("../components/StoreDebugPanel.vue"),
  },
  {
    path: "/about",
    name: "about",
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/AboutView.vue"),
  },
  {
    path: "/about/info",
    name: "about-info",
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/AboutInfoView.vue"),
  },
  {
    path: "/about/user-guide",
    name: "user-guide",
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/UserGuideView.vue"),
  },
  {
    path: "/about/software-info",
    name: "software-info",
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/SoftwareInfoView.vue"),
  },
  {
    path: "/units",
    name: "units",
    component: () =>
      import(/* webpackChunkName: "admin" */ "../views/UnitsView.vue"),
  },
  {
    path: "/positions",
    name: "positions",
    component: () =>
      import(/* webpackChunkName: "admin" */ "../views/PositionsView.vue"),
  },
  {
    path: "/roles",
    name: "roles",
    component: () =>
      import(/* webpackChunkName: "admin" */ "../views/RolesView.vue"),
  },
  {
    path: "/employees",
    name: "employees",
    component: () =>
      import(/* webpackChunkName: "employees" */ "../views/EmployeesView.vue"),
  },
  {
    path: "/khoan-periods",
    name: "khoan-periods",
    component: () =>
      import(/* webpackChunkName: "periods" */ "../views/KhoanPeriodsView.vue"),
  },
  {
    path: "/kpi-definitions",
    name: "kpi-definitions",
    component: () =>
      import(/* webpackChunkName: "kpi" */ "../views/KpiDefinitionsView.vue"),
  },
  {
    path: "/employee-kpi-assignment",
    name: "employee-kpi-assignment",
    component: () =>
      import(/* webpackChunkName: "kpi" */ "../views/EmployeeKpiAssignmentView.vue"),
  },
  {
    path: "/unit-kpi-assignment",
    name: "unit-kpi-assignment",
    component: () =>
      import(/* webpackChunkName: "kpi" */ "../views/UnitKpiAssignmentView.vue"),
  },
  {
    path: "/kpi-actual-values",
    name: "kpi-actual-values",
    component: () =>
      import(/* webpackChunkName: "kpi" */ "../views/KpiActualValuesView.vue"),
    meta: { public: true }, // Temporarily allow access without authentication for debugging
  },
  {
    path: "/kpi-scoring",
    name: "kpi-scoring",
    component: () =>
      import(/* webpackChunkName: "kpi" */ "../views/KpiScoringView.vue"),
  },
  {
    path: "/unit-kpi-scoring",
    name: "unit-kpi-scoring",
    component: () =>
      import(/* webpackChunkName: "kpi" */ "../views/UnitKpiScoringView.vue"),
    meta: { public: true }, // Temporarily allow access for debugging
  },
  {
    path: "/data-import",
    name: "data-import",
    component: DataImportView,
    meta: { requiresAuth: true }
  },
  {
    path: "/payroll-report",
    name: "PayrollReport",
    component: () =>
      import(/* webpackChunkName: "reports" */ '../views/PayrollReportView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: "/debug-dropdown",
    name: "debug-dropdown",
    component: () =>
      import(/* webpackChunkName: "debug" */ "../components/DebugDropdown.vue"),
  },
  {
    path: "/performance-dashboard",
    name: "performance-dashboard",
    component: () =>
      import(/* webpackChunkName: "admin" */ "../components/PerformanceDashboard.vue"),
    meta: { requiresAuth: true }
  },
  {
    path: "/debug-api",
    name: "debug-api",
    component: () =>
      import(/* webpackChunkName: "debug" */ "../components/DebugAPIComponent.vue"),
  },
  // === DASHBOARD ROUTES ===
  {
    path: "/dashboard",
    name: "dashboard",
    redirect: "/dashboard/business-plan",
    meta: { requiresAuth: true }
  },
  {
    path: "/dashboard/target-assignment",
    name: "target-assignment",
    component: () =>
      import(/* webpackChunkName: "dashboard" */ "../views/dashboard/TargetAssignment.vue"),
    meta: { requiresAuth: true }
  },
  {
    path: "/dashboard/calculation",
    name: "calculation-dashboard",
    component: () =>
      import(/* webpackChunkName: "dashboard" */ "../views/dashboard/CalculationDashboard.vue"),
    meta: { requiresAuth: true }
  },
  {
    path: "/dashboard/business-plan",
    name: "business-plan-dashboard",
    component: () =>
      import(/* webpackChunkName: "dashboard" */ "../views/dashboard/BusinessPlanDashboard.vue"),
    meta: { requiresAuth: true }
  },
];

const router = createRouter({
  history: createWebHistory(), // Bỏ process.env.BASE_URL vì Vite không hỗ trợ biến này
  routes,
});

// Route guard: bảo vệ các route cần đăng nhập
router.beforeEach((to, from, next) => {
  // Tạm thời bypass authentication để debug - kiểm tra xem vấn đề có ở đây không
  const bypassAuth = process.env.NODE_ENV === 'development'; // Chỉ bypass trong development

  if (!to.meta.public && !bypassAuth && !isAuthenticated()) {
    console.log('Router guard: redirecting to login', {
      route: to.path,
      isAuth: isAuthenticated(),
      token: localStorage.getItem('token') ? 'exists' : 'missing'
    });
    next({ name: "login" });
  } else {
    console.log('Router guard: allowing access', {
      route: to.path,
      isAuth: isAuthenticated(),
      bypassAuth,
      public: to.meta.public
    });
    next();
  }
});

export default router;
