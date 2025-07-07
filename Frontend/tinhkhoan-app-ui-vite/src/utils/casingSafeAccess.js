/**
 * 🎯 TINH KHOAN APP - CASING UTILITIES
 * Utility functions để handle PascalCase/camelCase safely
 * Date: 07/07/2025
 */

/**
 * Safely get property value with PascalCase priority
 * @param {Object} obj - Object to get property from
 * @param {string} propName - Property name in PascalCase
 * @returns {any} Property value or null
 */
export function safeGet(obj, propName) {
  if (!obj || typeof obj !== 'object') return null;

  // Try PascalCase first (backend format)
  if (obj[propName] !== undefined) {
    return obj[propName];
  }

  // Fallback to camelCase
  const camelCase = propName.charAt(0).toLowerCase() + propName.slice(1);
  if (obj[camelCase] !== undefined) {
    return obj[camelCase];
  }

  return null;
}

/**
 * Safely get Id property
 * @param {Object} obj - Object to get Id from
 * @returns {number|string|null} Id value
 */
export function getId(obj) {
  return safeGet(obj, 'Id');
}

/**
 * Safely get Name property
 * @param {Object} obj - Object to get Name from
 * @returns {string|null} Name value
 */
export function getName(obj) {
  return safeGet(obj, 'Name');
}

/**
 * Safely get Type property
 * @param {Object} obj - Object to get Type from
 * @returns {string|null} Type value
 */
export function getType(obj) {
  return safeGet(obj, 'Type');
}

/**
 * Safely get Status property
 * @param {Object} obj - Object to get Status from
 * @returns {string|null} Status value
 */
export function getStatus(obj) {
  return safeGet(obj, 'Status');
}

/**
 * Convert object properties to PascalCase
 * @param {Object} obj - Object to convert
 * @returns {Object} Object with PascalCase properties
 */
export function toPascalCase(obj) {
  if (!obj || typeof obj !== 'object') return obj;

  const result = {};

  for (const [key, value] of Object.entries(obj)) {
    // Convert camelCase to PascalCase
    const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
    result[pascalKey] = value;
  }

  return result;
}

/**
 * Ensure array items have consistent PascalCase properties
 * @param {Array} items - Array of objects
 * @returns {Array} Array with normalized objects
 */
export function normalizeArray(items) {
  if (!Array.isArray(items)) return [];

  return items.map(item => {
    if (!item || typeof item !== 'object') return item;

    return {
      ...item,
      // Ensure critical properties are available in PascalCase
      Id: getId(item),
      Name: getName(item),
      Type: getType(item),
      Status: getStatus(item),
    };
  });
}

/**
 * Safe dropdown option mapping
 * @param {Array} items - Array of items for dropdown
 * @param {string} valueField - Field name for option value (PascalCase)
 * @param {string} textField - Field name for option text (PascalCase)
 * @returns {Array} Normalized dropdown options
 */
export function mapDropdownOptions(items, valueField = 'Id', textField = 'Name') {
  if (!Array.isArray(items)) return [];

  return items
    .filter(item => item && typeof item === 'object')
    .map(item => ({
      value: safeGet(item, valueField),
      text: safeGet(item, textField),
      original: item // Keep reference to original object
    }))
    .filter(option => option.value !== null && option.text !== null);
}

/**
 * Debug helper to show available properties
 * @param {Object} obj - Object to debug
 * @param {string} context - Context description
 */
export function debugProperties(obj, context = 'Object') {
  if (!obj || typeof obj !== 'object') {
    console.log(`🔍 ${context}: null or not an object`);
    return;
  }

  const props = Object.keys(obj);
  console.log(`🔍 ${context} properties:`, props);

  // Show critical properties
  const critical = ['Id', 'id', 'Name', 'name', 'Type', 'type', 'Status', 'status'];
  critical.forEach(prop => {
    if (obj[prop] !== undefined) {
      console.log(`  ✅ ${prop}: ${obj[prop]}`);
    }
  });
}

// Export all utilities
export default {
  safeGet,
  getId,
  getName,
  getType,
  getStatus,
  toPascalCase,
  normalizeArray,
  mapDropdownOptions,
  debugProperties
};
