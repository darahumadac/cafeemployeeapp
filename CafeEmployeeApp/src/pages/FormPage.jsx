import React, { useEffect, useState } from "react";
import axios from "axios";
import { useParams, useLocation } from "react-router-dom";
import { API_URL } from "../../config.js";
import { TextField, MenuItem, Button, Box, Typography } from "@mui/material";
import Alert from "@mui/material/Alert";
import { Link } from "react-router-dom";

const validators = {
  name: {
    isValid: (value) => value && value.length >= 6 && value.length <= 10,
    helperText: "Must be minimum of 6 characters and maximum of 10 characters",
  },
  description: {
    isValid: (value) => value && value.length > 0 && value.length <= 256,
    helperText: "Must be maximum of 256 characters",
  },
  email: {
    isValid: (value) =>
      value && !/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(value),
    helperText: "Invalid email address",
  },
};

const FormPage = ({ title, populate = false }) => {
  // console.log(populate);
  const { pathname } = useLocation();
  const ADD_URL = `${API_URL}${pathname}`;

  useEffect(() => {
    populate &&
      axios
        .get(`${API_URL}${pathname}`)
        .then((response) => {
          const data = response.data;
          console.log(data);
        })
        .catch((err) => console.log(err, "error loading"));
  });

  const { id } = useParams();
  const clearForm = {
    name: { value: "", isValid: true },
    description: { value: "", isValid: true },
    location: { value: "", isValid: true },
  };

  const [formData, setFormData] = useState(clearForm);

  const [statusAlert, setStatusAlert] = useState({
    severity: "",
    message: "",
    show: false,
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: {
        ...formData[name],
        value,
        isValid: !validators[name] || validators[name].isValid(value),
      },
    });
    console.log(formData[name].isValid);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    //validate fields
    // Object.keys(formData).forEach(field => {
    //     setFormData((prev) => ({
    //         ...prev,
    //         [name]: {...prev[name], isValid: }
    //     }));
    // })
    if (Object.keys(formData).some((field) => !formData[field].isValid)) {
      return;
    }
    console.log("Submitted User Info:", formData);
    axios
      .post(`${ADD_URL}`, {
        name: formData.name.value,
        description: formData.description.value,
        location: formData.location.value,
      })
      .then((response) => {
        showAlert("success", "Successfully added");
        //clear fields
        clearFormData();
      })
      .catch((err) => showAlert("error", err.response.data.detail));
  };

  const showAlert = (severity, message) => {
    setStatusAlert({ severity, message, show: true });
  };

  const clearFormData = () => {
    setFormData(clearForm);
  };

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{ display: "flex", flexDirection: "column", gap: 2, width: 300 }}
    >
      <Button to="/cafes" component={Link}>
        Go back to cafes
      </Button>
      {statusAlert.show && (
        <Alert
          severity={statusAlert.severity}
          onClose={() => setStatusAlert({ ...statusAlert, show: false })}
        >
          {statusAlert.message}
        </Alert>
      )}
      <Typography variant="h6">{title} Form</Typography>
      <TextField
        label="Name"
        name="name"
        value={formData.name.value}
        onChange={handleChange}
        fullWidth
        required
        error={!formData.name.isValid}
        helperText={!formData.name.isValid && validators["name"].helperText}
      />
      <TextField
        label="Description"
        name="description"
        type="description"
        value={formData.description.value}
        onChange={handleChange}
        fullWidth
        required
        error={!formData.description.isValid}
        helperText={
          !formData.description.isValid && validators["description"].helperText
        }
      />
      <TextField
        label="Location"
        name="location"
        value={formData.location.value}
        onChange={handleChange}
        fullWidth
        required
      ></TextField>
      <Button type="submit" variant="contained">
        Submit
      </Button>
    </Box>
  );
};

export default FormPage;