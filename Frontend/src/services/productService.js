import api from './api';

const PRODUCTS_ENDPOINT = '/products';

export const productService = {
  // Get all products
  getAllProducts: async () => {
    try {
      const response = await api.get(PRODUCTS_ENDPOINT);
      return response.data;
    } catch (error) {
      console.error('Error fetching products:', error);
      throw error;
    }
  },

  // Get product by ID
  getProductById: async (id) => {
    try {
      const response = await api.get(`${PRODUCTS_ENDPOINT}/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching product ${id}:`, error);
      throw error;
    }
  },

  // Create new product
  createProduct: async (productData) => {
    try {
      const response = await api.post(PRODUCTS_ENDPOINT, productData);
      return response.data;
    } catch (error) {
      console.error('Error creating product:', error);
      throw error;
    }
  },

  // Update product
  updateProduct: async (id, productData) => {
    try {
      const response = await api.put(`${PRODUCTS_ENDPOINT}/${id}`, {
        ...productData,
        id: id
      });
      return response.data;
    } catch (error) {
      console.error(`Error updating product ${id}:`, error);
      throw error;
    }
  },

  // Delete product
  deleteProduct: async (id) => {
    try {
      await api.delete(`${PRODUCTS_ENDPOINT}/${id}`);
      return true;
    } catch (error) {
      console.error(`Error deleting product ${id}:`, error);
      throw error;
    }
  },
};
