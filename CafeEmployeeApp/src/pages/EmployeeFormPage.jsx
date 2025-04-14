import React, { useEffect, useRef, useState } from "react";
import axios from "axios";
import { useParams, useLocation } from "react-router-dom";
import { API_URL } from "../../config.js";
import {
  TextField,
  MenuItem,
  Button,
  Box,
  Typography,
  RadioGroup,
  FormControlLabel,
  Radio,
  FormLabel,
} from "@mui/material";
import Alert from "@mui/material/Alert";
import { Link, useNavigate } from "react-router-dom";
import Dropdown from "../Dropdown.jsx";

//TODO: refactor
const validators = {
  name: {
    isValid: (value) => value && value.length >= 6 && value.length <= 10,
    helperText: "Must be minimum of 6 characters and maximum of 10 characters",
  },
  emailAddress: {
    isValid: (value) =>
      value && /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(value),
    helperText: "Invalid email address",
  },
  phoneNumber: {
    isValid: (value) => value && /^(8|9)[0-9]{7}$/.test(value),
    helperText: "Invalid SG phone number",
  }
};

const EmployeeFormPage = ({ title, populate = false, entity }) => {
  // console.log(populate);
  const { pathname } = useLocation();
  const navigate = useNavigate();

  const ADD_URL = `${API_URL}${pathname}`;
  const UPDATE_URL = ADD_URL.replace(
    `${API_URL}/${entity}s`,
    `${API_URL}/${entity}`
  );

  const { id } = useParams();
  const clearForm = {
    name: { value: "", isValid: true },
    emailAddress: { value: "", isValid: true },
    phoneNumber: { value: "", isValid: true },
    gender: { value: true, isValid: true }
  };

  const [formData, setFormData] = useState(clearForm);
  const [isFormDirty, setIsFormDirty] = useState(false);
  const [etag, setEtag] = useState("");
  const cafeRef = useRef(null);
  const [selected, setSelected] = useState("Not Assigned");
  const [cafes, setCafes] = useState([]);

  useEffect(() => {
    //TODO: implement location endpoint
    axios.get(`${API_URL}/cafes`).then((response) => {
      const allCafes = response.data;
      cafeRef.current = allCafes;
      setCafes(allCafes.map(c => c.name));
    });
  }, []);

  useEffect(() => {
    populate &&
      axios
        .get(`${API_URL}${pathname}`)
        .then((response) => {
          const data = response.data;
          const currentData = Object.entries(data)
            .map(([key, value]) => {
              if (!value) return;
              return { [key]: { value, isValid: true } };
            })
            .filter((o) => o);

          const currentFormData = Object.fromEntries(
            currentData.map((d) => {
              return [Object.keys(d)[0], Object.values(d)[0]];
            })
          );
          setFormData(currentFormData);
          setEtag(response.headers["etag"]);
        })
        .catch((err) => console.log(err, "error loading"));
  }, []);

  const [statusAlert, setStatusAlert] = useState({
    severity: "",
    message: "",
    show: false,
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setIsFormDirty(true);
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
    if (Object.keys(formData).some((field) => !formData[field].isValid)) {
      console.log(formData)
      return;
    }
    console.log("Submitted User Info:", formData);

    if (populate) {
      axios
        .put(
          `${UPDATE_URL}`,
          {
            name: formData.name.value,
            description: formData.description.value,
            location: formData.location.value,
          },
          {
            headers: {
              "If-Match": `${etag}`,
            },
          }
        )
        .then((response) => {
          showAlert("success", "Updated successfully");
          setEtag(response.headers["etag"]);
          setIsFormDirty(false);
        })
        .catch((err) => {
          let errMsg = err.response.data.detail || "Something went wrong";
          if (err.response.status == 412) {
            errMsg =
              "Someone else may have edited this item while you were working. Please reload the page to see the latest version.";
          }
          showAlert("error", errMsg);
          // console.log(err.response)
        });

      return;
    }

    console.log(formData.gender.value)

    axios
      .post(`${ADD_URL}`, {
        name: formData.name.value,
        emailAddress: formData.emailAddress.value,
        phoneNumber: formData.phoneNumber.value,
        gender: Number(formData.gender.value),
        assignedCafeId: cafeRef.current.filter(c => c.name == selected)[0].id
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

  const handleCafeChange = (e) => {
    console.log("cafe change", e.target.value);
    setSelected(e.target.value)
  };


  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{ display: "flex", flexDirection: "column", gap: 2, width: 300 }}
    >
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
        label="Email Address"
        name="emailAddress"
        value={formData.emailAddress.value}
        onChange={handleChange}
        fullWidth
        required
        error={!formData.emailAddress.isValid}
        helperText={
          !formData.emailAddress.isValid &&
          validators["emailAddress"].helperText
        }
      />
      <TextField
        label="Phone Number"
        name="phoneNumber"
        value={formData.phoneNumber.value}
        onChange={handleChange}
        fullWidth
        required
        error={!formData.phoneNumber.isValid}
        helperText={
          !formData.phoneNumber.isValid && validators["phoneNumber"].helperText
        }
      ></TextField>
      <FormLabel>Gender</FormLabel>
      <RadioGroup defaultValue={0} name="gender" onChange={handleChange}>
        <FormControlLabel value={0} control={<Radio />} label="Male" />
        <FormControlLabel value={1} control={<Radio />} label="Female" />
      </RadioGroup>
      <Dropdown
        defaultSelection={"Not Assigned"}
        label={"Location"}
        items={cafes}
        selected={selected}
        setSelected={setSelected}
        handleChange={handleCafeChange}
      />
      <Button type="submit" variant="contained">
        Submit
      </Button>
      <Button
        onClick={() => {
          if (isFormDirty) {
            confirm(
              "You have unsaved changes. Are you sure you want to leave this page?"
            ) && navigate(-1);
          } else {
            navigate(-1);
          }
        }}
      >
        Cancel
      </Button>
    </Box>
  );
};

export default EmployeeFormPage;
